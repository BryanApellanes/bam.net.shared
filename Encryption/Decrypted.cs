/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;

namespace Bam.Net.Encryption
{
    public class Decrypted: Encrypted
    {
        public Decrypted(byte[] cipher, byte[] key, byte[] iv)
            : base(cipher, key, iv)
        {
        }

        public Decrypted(Encrypted value)
            : base(value.Cipher, value.Key, value.IV)
        {
            this.SaltLength = value.SaltLength;
            Decrypt();
        }

        public Decrypted(string base64Cipher, string b64Key, string iv)
        {
            this.Base64Cipher = base64Cipher;
            this.Base64Key = b64Key;
            this.Base64IV = iv;
        }

        public Decrypted(string base64Cipher, AesKeyVectorPair aesKeyVectorPair) 
            : this(base64Cipher.FromBase64(), aesKeyVectorPair.Key.FromBase64(), aesKeyVectorPair.IV.FromBase64())
        { 
        }

        public static implicit operator string(Decrypted dec)
        {
            return dec.Value;
        }

        public override string Value
        {
            get
            {
                if (string.IsNullOrEmpty(Plain))
                {
                    Decrypt();
                }

                return Plain;
            }
        }

        protected string Decrypt()
        {
            Plain = Decrypt(Cipher, Key, IV).Truncate(SaltLength);
            return Plain;
        }

        public static string Decrypt(byte[] cipher, byte[] key, byte[] iv)
        {
            return Aes.Decrypt(Convert.ToBase64String(cipher), Convert.ToBase64String(key), Convert.ToBase64String(iv));
        }
    }
}
