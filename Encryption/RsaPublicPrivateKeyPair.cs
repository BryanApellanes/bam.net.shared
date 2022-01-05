using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaPublicPrivateKeyPair
    {
        public RsaPublicPrivateKeyPair(RsaKeyLength rsaKeyLength = RsaKeyLength._2048)
        {
            this.RsaKeyLength = rsaKeyLength;
            this.Encoding = Encoding.UTF8;
            this.AsymmetricCipherKeyPair = Rsa.GenerateKeyPair(rsaKeyLength);
            this.Pem = AsymmetricCipherKeyPair.ToPem();
            this.PublicKeyPem = AsymmetricCipherKeyPair.PublicKeyToPem();
        }

        public RsaPublicPrivateKeyPair(string pemString)
        {
            this.RsaKeyLength = RsaKeyLength.Unkown;
            this.Pem = pemString;
            this.AsymmetricCipherKeyPair = pemString.ToKeyPair();
            this.PublicKeyPem = AsymmetricCipherKeyPair.PublicKeyToPem();
        }

        AsymmetricCipherKeyPair _asymmetricCipherKeyPair;
        protected AsymmetricCipherKeyPair AsymmetricCipherKeyPair
        {
            get
            {
                if(_asymmetricCipherKeyPair == null)
                {
                    _asymmetricCipherKeyPair = Pem.ToKeyPair();
                }

                return _asymmetricCipherKeyPair;
            }
            set
            {
                _asymmetricCipherKeyPair = value;
            }
        }

        public Encoding Encoding { get; set; }

        public RsaKeyLength RsaKeyLength { get; set; }

        /// <summary>
        /// Gets or sets the full keypair as a pem string.
        /// </summary>
        public string Pem { get; }

        public string PublicKeyPem { get; set; }

        /// <summary>
        /// Gets a base 64 encoded cipher for the specified plain text.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string Encrypt(string plainText)
        {
            byte[] plainData = Encoding.GetBytes(plainText);
            byte[] encrypted = Encrypt(plainData);
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Gets an encrypted byte array for the specified plain data.
        /// </summary>
        /// <param name="plainData"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] plainData)
        {
            return plainData.GetPublicKeyEncryptedBytes(_asymmetricCipherKeyPair.Public);
        }

        public string Decrypt(string base64Cipher)
        {
            byte[] cipherBytes = base64Cipher.FromBase64();
            byte[] decrypted = Decrypt(cipherBytes);
            return Encoding.GetString(decrypted);
        }

        public byte[] Decrypt(byte[] cipherBytes)
        {
            return cipherBytes.DecryptWithPrivateKey(AsymmetricCipherKeyPair.Private);
        }

    }
}
