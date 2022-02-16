/*
	Copyright © Bryan Apellanes 2015  
*/
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public static class Extensions
    {
        public static Email SetCredentials(this Email email, Vault credentialVault)
        {            
            email.SmtpHost(credentialVault["SmtpHost"])
                .UserName(credentialVault["UserName"])
                .Password(credentialVault["Password"]);

            return email;
        }

        public static string AesPasswordEncrypt(this string plainText, string password)
        {
            return new PasswordEncrypted(plainText, password);            
        }

        public static string AesPasswordDecrypt(this string cipher, string password)
        {
            return new PasswordDecrypted(cipher, password);            
        }
        
        public static AsymmetricCipherKeyPair RsaKeyPair(this RsaKeyLength size, string secureRandomAlgorithm = "SHA1PRNG")
        {
            return RsaKeyPair((int)size, secureRandomAlgorithm);
        }

        public static AsymmetricCipherKeyPair RsaKeyPair(this int size, string secureRandomAlgorithm = "SHA1PRNG")
        {
            RsaKeyPairGenerator gen = new RsaKeyPairGenerator();
            RsaKeyGenerationParameters parameters = new RsaKeyGenerationParameters(new BigInteger("10001", 16), SecureRandom.GetInstance(secureRandomAlgorithm), size, 80);
            gen.Init(parameters);
            return gen.GenerateKeyPair();
        }
        
        /// <summary>
        /// Gets a base 64 encoded asymmetric cipher of the specified input.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="publicPemKey"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string EncryptWithPublicKey(this string input, string publicPemKey, Encoding encoding = null)
        {
            return EncryptWithPublicKey(input, publicPemKey.ToKey(), encoding);
        }
        
        /// <summary>
        /// Gets a base 64 encoded asymmetric cipher of the specified plain text input.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="encoding"></param>
        /// <param name="engine"></param>
        /// <returns></returns>
        public static string EncryptWithPublicKey(this string plainText, AsymmetricKeyParameter key, Encoding encoding = null, IAsymmetricBlockCipher engine = null)
        {
            byte[] encrypted = GetPublicKeyEncryptedBytes(plainText, key, encoding, engine);
            return Convert.ToBase64String(encrypted);
        }

        public static byte[] GetPublicKeyEncryptedBytes(this string plainText, AsymmetricKeyParameter key, Encoding encoding = null, IAsymmetricBlockCipher engine = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            byte[] plainData = encoding.GetBytes(plainText);
            byte[] encrypted = plainData.AsymmetricEncrypt(key, engine);
            return encrypted;
        }

        public static byte[] GetPublicKeyEncryptedBytes(this byte[] plainData, string publicPemKey, IAsymmetricBlockCipher engine = null)
        {
            return GetPublicKeyEncryptedBytes(plainData, publicPemKey.ToKey(), engine);
        }

        public static byte[] GetPublicKeyEncryptedBytes(this byte[] plainData, AsymmetricKeyParameter key, IAsymmetricBlockCipher engine = null)
        {
            byte[] encrypted = plainData.AsymmetricEncrypt(key, engine);
            return encrypted;
        }

        public static string DecryptWithPrivateKey(this string base64EncodedCipher, AsymmetricCipherKeyPair keys, Encoding encoding = null)
        {
            return DecryptWithPrivateKey(base64EncodedCipher, keys.Private, encoding, false);
        }

        public static string DecryptWithPrivateKey(this string base64EncodedCipher, AsymmetricKeyParameter privateKey, Encoding encoding = null, bool usePkcsPadding = false)
        {
            return DecryptWithPrivateKey(base64EncodedCipher, privateKey, encoding, Rsa.GetRsaEngine(usePkcsPadding));
        }

        public static string DecryptWithPrivateKey(this string base64EncodedCipher, AsymmetricKeyParameter privateKey, Encoding encoding = null, IAsymmetricBlockCipher engine = null)
        {
            byte[] decrypted = GetPrivateKeyDecryptedBytes(base64EncodedCipher, privateKey, engine);
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetString(decrypted);
        }

        public static byte[] GetPrivateKeyDecryptedBytes(this string base64EncodedCipher, AsymmetricKeyParameter privateKey, IAsymmetricBlockCipher engine)
        {
            byte[] encrypted = Convert.FromBase64String(base64EncodedCipher);
            byte[] decrypted = DecryptWithPrivateKey(encrypted, privateKey, engine);
            return decrypted;
        }

        public static string DecryptWithPrivateKey(this string base64EncodedCipher, string pemString, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return DecryptWithPrivateKey(base64EncodedCipher, pemString.ToKeyPair(), encoding);
        }

        /// <summary>
        /// Encrypt with the Public key of the specified keyPair
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="keyPair"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string EncryptWithPublicKey(this string plainText, AsymmetricCipherKeyPair keyPair, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            byte[] data = encoding.GetBytes(plainText);
            byte[] encrypted = data.EncryptWithPublicKey(keyPair);
            return Convert.ToBase64String(encrypted);
        }

        public static byte[] EncryptWithPublicKey(this byte[] plainData, AsymmetricCipherKeyPair keyPair)
        {
            return AsymmetricEncrypt(plainData, keyPair.Public);
        }

        public static byte[] AsymmetricEncrypt(this byte[] plainData, AsymmetricKeyParameter key, bool usePkcsPadding = false)
        {
            return AsymmetricEncrypt(plainData, key, Rsa.GetRsaEngine(usePkcsPadding));
        }

        public static byte[] AsymmetricEncrypt(this byte[] plainData, AsymmetricKeyParameter key, IAsymmetricBlockCipher e)
        {
            if (e == null)
            {
                e = new RsaEngine();
            }

            e.Init(true, key);

            int blockSize = e.GetInputBlockSize();
            List<byte> output = new List<byte>();
            for (int chunkPosition = 0; chunkPosition < plainData.Length; chunkPosition += blockSize)
            {
                int chunkSize = Math.Min(blockSize, plainData.Length - (chunkPosition * blockSize));
                output.AddRange(e.ProcessBlock(plainData, chunkPosition, chunkSize));
            }

            return output.ToArray();
        }

        public static byte[] DecryptWithPrivateKey(this byte[] byteArrayCipher, AsymmetricKeyParameter key, IAsymmetricBlockCipher engine = null)
        {
            if (engine == null)
            {
                engine = new RsaEngine();
            }

            engine.Init(false, key);

            int blockSize = engine.GetInputBlockSize();

            List<byte> output = new List<byte>();
            for (int chunkPosition = 0; chunkPosition < byteArrayCipher.Length; chunkPosition += blockSize)
            {
                int chunkSize = Math.Min(blockSize, byteArrayCipher.Length - (chunkPosition * blockSize));
                output.AddRange(engine.ProcessBlock(byteArrayCipher, chunkPosition, chunkSize));
            }

            return output.ToArray();
        }
    }
}
