﻿using Bam.Net.Data.Repositories;
using Bam.Net.Encryption.Data.Files;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bam.Net.Encryption.Data
{
    public class KeySet : KeyedAuditRepoData, IKeySet
    {
        public KeySet() 
        {
            this.RsaKeyLength = RsaKeyLength._2048;
        }
        
        public KeySet(RsaKeyLength keyLength = RsaKeyLength._2048)
        {
            this.RsaKeyLength = keyLength;
        }

        public KeySet(RsaKeyLength keyLength, bool init = false) : this(keyLength)
        {
            this.Identifier = $"uninitialized_".RandomLetters(8);
            if (init)
            {
                Init();
            }
        }

        [CompositeKey]
        public string Identifier { get; set; }

        public RsaKeyLength RsaKeyLength { get; set; }

        public string RsaKey { get; set; }

        public string AesKey { get; set; }

        public string AesIV { get; set; }

        public string Secret { get; set; }

        public string GetSecret()
        {
            return this.Secret;
        }

        /// <inheritdoc />
        public string Encrypt(string value)
        {
            return Aes.Encrypt(value, GetAesKeyVectorPair());
        }

        public string Decrypt(string base64EncodedValue)
        {
            return Aes.Decrypt(base64EncodedValue, GetAesKeyVectorPair());
        }

        public string AsymmetricEncrypt(string plainText, IAsymmetricBlockCipher engine = null)
        {
            return PublicKeyEncrypt(plainText, engine);
        }

        /// <summary>
        /// Use private key to decrypt, same as PrivateKeyDecrypt.
        /// </summary>
        /// <param name="cipher">The cipher.</param>
        /// <param name="engine">The engine.</param>
        /// <returns></returns>
        public string AsymmetricDecrypt(string cipher, IAsymmetricBlockCipher engine = null)
        {
            return PrivateKeyDecrypt(cipher, engine);
        }

        /// <summary>
        /// Use public key to encrypt, same as AsymmetricEncrypt.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="engine">The engine.</param>
        /// <returns></returns>
        public string PublicKeyEncrypt(string plainText, IAsymmetricBlockCipher engine = null)
        {
            AsymmetricKeyParameter key = GetAsymmetricKeys().Public;

            return plainText.EncryptWithPublicKey(key, null, engine);
        }

        public string PrivateKeyDecrypt(string cipher, IAsymmetricBlockCipher engine = null)
        {
            return cipher.DecryptWithPrivateKey(RsaKey.ToKeyPair().Private, null, engine);
        }

        AesKeyVectorPair _aesKeyVectorPair;
        public AesKeyVectorPair GetAesKeyVectorPair()
        {
            if (_aesKeyVectorPair == null)
            {
                _aesKeyVectorPair = new AesKeyVectorPair { Key = AesKey, IV = AesIV };
            }

            return _aesKeyVectorPair;
        }

        AsymmetricCipherKeyPair _asymmetricCipherKeyPair;
        public AsymmetricCipherKeyPair GetAsymmetricKeys()
        {
            if (_asymmetricCipherKeyPair == null)
            {
                _asymmetricCipherKeyPair = RsaKey.ToKeyPair();
            }
            return _asymmetricCipherKeyPair;
        }

        protected void Init()
        {
            AsymmetricCipherKeyPair rsaKeyPair = Rsa.GenerateKeyPair(RsaKeyLength);
            Identifier = $"keyset_{rsaKeyPair.PublicKeyToPem().Sha256()}";

            RsaKey = rsaKeyPair.ToPem();

            AesKeyVectorPair akvp = new AesKeyVectorPair();
            AesKey = akvp.Key;
            AesIV = akvp.IV;
        }
    }
}