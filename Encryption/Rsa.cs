/*
	Copyright © Bryan Apellanes 2015  
*/
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Encryption
{
    public static class Rsa
    {
        static Rsa()
        {
            DefaultKeySize = 1024;
        }

        public static int DefaultKeySize { get; set; }

        public static string Encrypt(string value)
        {
            return RsaKeyFile.Default.EncryptWithPublicKey(value);
        }

        public static string Decrypt(string cipherText)
        {
            return RsaKeyFile.Default.DecryptWithPrivateKey(cipherText);
        }

        public static string GetPublicKey()
        {
            return RsaKeyFile.Default.PublicKeyXml;
        }

        public static AsymmetricCipherKeyPair GenerateKeyPair(RsaKeyLength size)
        {
            return size.RsaKeyPair();
        }

        public static IAsymmetricBlockCipher GetRsaEngine(bool usePkcsPadding)
        {
            IAsymmetricBlockCipher result = new RsaEngine();
            if (usePkcsPadding)
            {
                result = new Pkcs1Encoding(result); // wrap the engine in a padded encoding
            }

            return result;
        }
    }
}
