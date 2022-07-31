using Bam.Net.Data.Repositories;
using Bam.Net.Encryption;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Encryption;
using Bam.Net.Web;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Data
{
    [Encrypt]
    [Serializable]
    public class SecureChannelSession : KeyedAuditRepoData
    {
        public const string CookieName = "bam-scs-id";

        public SecureChannelSession()
        {
            IdentifierHashAlgorithm = HashAlgorithms.SHA256;
            ValidationAlgorithm = HashAlgorithms.SHA256;
            IdentifierSeedLength = 128;
            DateTime dateTimeNow = DateTime.UtcNow;
            Created = dateTimeNow;
            TimeOffset = 0;
            _ = TouchAsync();
        }

        public SecureChannelSession(Instant clientNow, bool initializeAsymmetricKey = false, RsaKeyLength rsaKeyLength = RsaKeyLength._2048) : this()
        {
            TimeOffset = clientNow.DiffInMilliseconds(DateTime.UtcNow);
            if (initializeAsymmetricKey)
            {
                InitializeAsymmetricKey(rsaKeyLength);
            }
        }

        AsymmetricCipherKeyPair _asymmetricCipherKeyPair;
        protected AsymmetricCipherKeyPair AsymmetricCipherKeyPair
        {
            get
            {
                if (_asymmetricCipherKeyPair == null)
                {
                    _asymmetricCipherKeyPair = AsymmetricKey.ToKeyPair();
                }
                return _asymmetricCipherKeyPair;
            }
        }

        protected AsymmetricKeyParameter GetPrivateKey()
        {
            return AsymmetricCipherKeyPair.Private;
        }

        [CompositeKey]
        public HashAlgorithms IdentifierHashAlgorithm { get; set; }

        [CompositeKey]
        public string Identifier { get; set; }

        public HashAlgorithms ValidationAlgorithm { get; set; }

        public string AsymmetricKey { get; set; }

        /// <summary>
        /// Gets or sets the public key encrypted cipher of the AES key.
        /// </summary>
        public string SymmetricKey { get; set; }

        /// <summary>
        /// Gets or sets the public key encrypted cipher of the AES initialization vector.
        /// </summary>
        public string SymmetricIV { get; set; }

        /// <summary>
        /// Gets or sets the difference in milliseconds between the client's clock and the server's clock including request latency.
        /// </summary>
        public int? TimeOffset { get; set; }

        /// <summary>
        /// Gets or sets when the last activity for this session was recorded.
        /// </summary>
        public DateTime? LastActivity { get; set; }

        /// <summary>
        /// Gets or sets the value indicating when this session expires.
        /// </summary>
        public DateTime? Expires { get; set; }

        public string Server
        {
            get;
            set;
        }

        public string Client
        {
            get;
            set;
        }

        /// <summary>
        /// Sets LastActivity and sets Expires to the specified number of minutes in the future.
        /// </summary>
        /// <param name="expiresInMinutes"></param>
        /// <returns></returns>
        public async Task TouchAsync(int expiresInMinutes = 15)
        {
            await Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(SymmetricKey) && !string.IsNullOrEmpty(SymmetricIV))
                {
                    this.Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);

                    _ = UpdateLastActivityAsync();
                }
            });
        }

        public string DecryptWithPrivateKey(string cipher, bool usePkcsPadding)
        {
            return DecryptWithPrivateKey(cipher, null, usePkcsPadding);
        }

        public string DecryptWithPrivateKey(string cipher, Encoding encoding, bool usePkcsPadding)
        {
            if (string.IsNullOrEmpty(cipher))
            {
                throw new ArgumentNullException(nameof(cipher));
            }
            _ = TouchAsync();
            return cipher.DecryptWithPrivateKey(AsymmetricCipherKeyPair.Private, encoding, Rsa.GetRsaEngine(usePkcsPadding));
        }

        public string GetPublicKey()
        {
            return AsymmetricCipherKeyPair.Public.ToPem();
        }

        public SecureChannelSession InitializeAsymmetricKey(RsaKeyLength rsaKeyLength = RsaKeyLength._2048)
        {
            SetIdentifier();
            AsymmetricKey = Rsa.GenerateKeyPair(rsaKeyLength).ToPem();
            return this;
        }

        public ClientSessionInfo GetClientSession(bool throwIfMissingKeys = true)
        {
            if (throwIfMissingKeys && string.IsNullOrEmpty(AsymmetricKey))
            {
                throw new AsymmetricKeyNotSetException(this);
            }
            if (throwIfMissingKeys && (string.IsNullOrEmpty(SymmetricIV) || string.IsNullOrEmpty(SymmetricKey)))
            {
                throw new SessionKeyNotSetException(this);
            }

            _ = TouchAsync();
            return new ClientSessionInfo
            {
                ClientSessionIdentifier = Identifier,
                PublicKey = GetPublicKey(),
                AesIV = SymmetricIV.DecryptWithPrivateKey(AsymmetricCipherKeyPair, Encoding.UTF8),
                AesKey = SymmetricKey.DecryptWithPrivateKey(AsymmetricCipherKeyPair, Encoding.UTF8),
            };
        }

        internal void SetSymmetricKey(SetSessionKeyRequest setSessionKeyRequest, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            SymmetricKey = setSessionKeyRequest.KeyCipher;
            string plainSymmetricKey = SymmetricKey.DecryptWithPrivateKey(GetPrivateKey(), encoding, setSessionKeyRequest.GetEngine());
            string keyHash = setSessionKeyRequest.KeyHashCipher.DecryptWithPrivateKey(GetPrivateKey(), encoding, setSessionKeyRequest.GetEngine());
            Expect.AreEqual(plainSymmetricKey.HashHexString(ValidationAlgorithm), keyHash, "Key hash check failed");

            SymmetricIV = setSessionKeyRequest.IVCipher;
            string plainSymmetricIV = SymmetricIV.DecryptWithPrivateKey(GetPrivateKey(), encoding, setSessionKeyRequest.GetEngine());
            string ivHash = setSessionKeyRequest.IVHashCipher.DecryptWithPrivateKey(GetPrivateKey(), encoding, setSessionKeyRequest.GetEngine());
            Expect.AreEqual(plainSymmetricIV.HashHexString(ValidationAlgorithm), ivHash, "IV hash check failed");

            _ = TouchAsync();
        }

        internal int IdentifierSeedLength { get; set; }

        private void SetIdentifier()
        {
            SecureRandom secureRandom = new SecureRandom();
            Identifier = secureRandom.GenerateSeed(IdentifierSeedLength).ToBase64().HashHexString(IdentifierHashAlgorithm);
        }

        private async Task UpdateLastActivityAsync()
        {
            await Task.Run(() =>
            {
                LastActivity = DateTime.UtcNow;
                if (Repository != null)
                {
                    Repository.Save(this);
                }
            });            
        }
    }
}
