using Bam.Net.Data.Repositories;
using Bam.Net.Encryption;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Web;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server.ServiceProxy.Data
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
            LastActivity = dateTimeNow;
            TimeOffset = 0;
        }

        public SecureChannelSession(Instant clientNow, bool initialize = false, RsaKeyLength rsaKeyLength = RsaKeyLength._2048) : this()
        {
            TimeOffset = clientNow.DiffInMilliseconds(DateTime.UtcNow);
            if (initialize)
            {
                Initialize(rsaKeyLength);
            }
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

        public DateTime? LastActivity { get; set; }

        AsymmetricCipherKeyPair _asymmetricCipherKeyPair;
        protected AsymmetricCipherKeyPair AsymmetricCipherKeyPair
        {
            get
            {
                if(_asymmetricCipherKeyPair == null)
                {
                    _asymmetricCipherKeyPair = AsymmetricKey.ToKeyPair();
                }
                return _asymmetricCipherKeyPair;
            }
        }

        public string DecryptWithPrivateKey(string cipher, bool usePkcsPadding)
        {
            if (string.IsNullOrEmpty(cipher))
            {
                throw new ArgumentNullException(nameof(cipher));
            }
            return cipher.DecryptWithPrivateKey(AsymmetricCipherKeyPair);
        }

        public string GetPublicKey()
        {
            return AsymmetricCipherKeyPair.Public.ToPem();
        }

        public SecureChannelSession Initialize(RsaKeyLength rsaKeyLength = RsaKeyLength._2048)
        {
            SetIdentifier();
            AsymmetricKey = Rsa.GenerateKeyPair(rsaKeyLength).ToPem();
            return this;
        }

        public ClientSessionInfo ToClientSessionInfo()
        {
            return new ClientSessionInfo
            {
                ClientIdentifier = Identifier,
                PublicKey = GetPublicKey(),
                SessionIV = this.SymmetricIV,
                SessionKey = this.SymmetricKey,
            };
        }

        internal void SetSymmetricKey(SetSessionKeyRequest setSessionKeyRequest, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            SymmetricKey = setSessionKeyRequest.KeyCipher.DecryptWithPrivateKey(GetPrivateKey(), encoding, setSessionKeyRequest.GetEngine());
            string keyHash = setSessionKeyRequest.KeyHashCipher.DecryptWithPrivateKey(GetPrivateKey(), encoding, setSessionKeyRequest.GetEngine());
            Expect.AreEqual(SymmetricKey.HashHexString(ValidationAlgorithm), keyHash, "Key hash check failed");
            SymmetricIV = setSessionKeyRequest.IVCipher.DecryptWithPrivateKey(GetPrivateKey(), encoding, setSessionKeyRequest.GetEngine());
            string ivHash = setSessionKeyRequest.IVHashCipher.DecryptWithPrivateKey(GetPrivateKey(), encoding, setSessionKeyRequest.GetEngine());
            Expect.AreEqual(SymmetricIV.HashHexString(ValidationAlgorithm), ivHash, "IV hash check failed");
        }

        protected AsymmetricKeyParameter GetPrivateKey()
        {
            return AsymmetricCipherKeyPair.Private;
        }

        internal int IdentifierSeedLength { get; set; }

        private void SetIdentifier()
        {
            SecureRandom secureRandom = new SecureRandom();
            Identifier = secureRandom.GenerateSeed(IdentifierSeedLength).ToBase64().HashHexString(IdentifierHashAlgorithm);
        }
    }
}
