using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaPublicPrivateKeyPair : IRsaKeySource
    {
        public RsaPublicPrivateKeyPair(RsaKeyLength rsaKeyLength = RsaKeyLength._2048)
        {
            this.RsaKeyLength = rsaKeyLength;
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

        public RsaKeyLength RsaKeyLength { get; set; }

        /// <summary>
        /// Gets the full keypair as a pem string.
        /// </summary>
        public string Pem { get; private set; }

        /// <summary>
        /// Gets the public key as a pem string.
        /// </summary>
        public string PublicKeyPem { get; private set; }

        /// <summary>
        /// Gets a base 64 encoded cipher for the specified plain text.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string Encrypt(string plainText, Encoding encoding = null)
        {
            byte[] plainData = (encoding ?? Encoding.UTF8).GetBytes(plainText);
            byte[] encrypted = EncryptBytes(plainData);
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Deciphers the specifed base 64 encoded cipher text.
        /// </summary>
        /// <param name="base64Cipher"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string Decrypt(string base64Cipher, Encoding encoding = null)
        {
            byte[] cipherBytes = base64Cipher.FromBase64();
            byte[] decrypted = DecryptBytes(cipherBytes);
            return (encoding ?? Encoding.UTF8).GetString(decrypted);
        }

        /// <summary>
        /// Using the public key gets an encrypted byte array for the specified plain data.
        /// </summary>
        /// <param name="plainData">The data to encrypt.</param>
        /// <param name="usePkcsPadding">A value indicating whether to use padding, the default is false.</param>
        /// <returns></returns>
        public byte[] EncryptBytes(byte[] plainData, bool usePkcsPadding)
        {
            return EncryptBytes(plainData, Rsa.GetRsaEngine(usePkcsPadding));
        }

        /// <summary>
        /// Using the public key gets an encrypted byte array for the specified plain data.
        /// </summary>
        /// <param name="plainData"></param>
        /// <returns></returns>
        public byte[] EncryptBytes(byte[] plainData, IAsymmetricBlockCipher engine = null)
        {
            return plainData.GetPublicKeyEncryptedBytes(_asymmetricCipherKeyPair.Public, engine);
        }
        
        /// <summary>
        /// Using the private key, gets an unencrypted byte array for the specified cipher.
        /// </summary>
        /// <param name="cipherBytes">The data to decrypt.</param>
        /// <param name="usePkcsPadding">A value indicating whether the cipher is pkcs padded.</param>
        /// <returns></returns>
        public byte[] DecryptBytes(byte[] cipherBytes, bool usePkcsPadding)
        {
            return DecryptBytes(cipherBytes, Rsa.GetRsaEngine(usePkcsPadding));
        }

        /// <summary>
        /// Using the private key, gets an unencrypted byte array for the specified cipher.
        /// </summary>
        /// <param name="cipherBytes"></param>
        /// <returns></returns>
        public byte[] DecryptBytes(byte[] cipherBytes, IAsymmetricBlockCipher engine = null)
        {
            return cipherBytes.DecryptWithPrivateKey(AsymmetricCipherKeyPair.Private, engine);
        }

        public RsaPublicKey GetRsaPublicKey()
        {
            return new RsaPublicKey(PublicKeyPem);
        }

        public RsaPublicPrivateKeyPair GetRsaKey()
        {
            return this;
        }
    }
}
