using Bam.Net.Encryption;
using Bam.Net.ServiceProxy.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class ValidationToken
    {
        public ValidationToken(int allowedOffsetMinutes = 3) 
        {
            this.AllowedOffsetMinutes = allowedOffsetMinutes;
        }

        public ValidationToken(string plainString, string nonce = null, HashAlgorithms hashAlgorithm = HashAlgorithms.SHA256)
        {
            this.Timestamp = nonce ?? new Instant().ToString();
            this.Algorithm = hashAlgorithm;
            this.Hash = GetHash(plainString);
        }

        public HashAlgorithms Algorithm { get; set; }
        public string Timestamp { get; set; }
        public string Hash { get; set; }

        public int AllowedOffsetMinutes { get; private set; }

        public string GetHash(string validatedString)
        {
            return $"{Timestamp}:{validatedString}".HashHexString(Algorithm);
        }

        public EncryptedValidationToken Encrypt(string publicKeyPem)
        {
            return new EncryptedValidationToken
            {
                HashCipher = Hash.EncryptWithPublicKey(publicKeyPem),
                TimestampCipher = Timestamp.EncryptWithPublicKey(publicKeyPem),
                Algorithm = Algorithm,
            };
        }
    }
}
