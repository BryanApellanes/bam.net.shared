using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaPublicKey
    {
        public RsaPublicKey(string publicKeyPem)
        {
            this.Pem = publicKeyPem;
        }

        /// <summary>
        /// Gets or sets the public key pem string.
        /// </summary>
        public string Pem { get; set; }

        public string Encrypt(string plainText, Encoding encoding = null)
        {
            byte[] plainData = (encoding ?? Encoding.UTF8).GetBytes(plainText);
            byte[] encrypted = EncryptBytes(plainData);
            return Convert.ToBase64String(encrypted);
        }

        public byte[] EncryptBytes(byte[] plainData, bool usePkcsPadding)
        {
            return EncryptBytes(plainData, Rsa.GetRsaEngine(usePkcsPadding));
        }

        public byte[] EncryptBytes(byte[] plainData, IAsymmetricBlockCipher engine = null)
        {
            return plainData.GetPublicKeyEncryptedBytes(this.Pem, engine);
        }
    }
}
