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
    [Serializable]
    public class SecureChannelSession : KeyedAuditRepoData
    {
        public const string CookieName = "bam-scs-id";

        public SecureChannelSession()
        {
            IdentifierHashAlgorithm = HashAlgorithms.SHA256;
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
        /// Gets or sets the difference in milliseconds between the client's clock and the server's clock.
        /// </summary>
        public int? TimeOffset { get; set; }

        public DateTime? LastActivity { get; set; }

        public string GetPublicKey()
        {
            return AsymmetricKey.ToKeyPair().Public.ToPem();
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

        internal int IdentifierSeedLength { get; set; }

        private void SetIdentifier()
        {
            SecureRandom secureRandom = new SecureRandom();
            Identifier = secureRandom.GenerateSeed(IdentifierSeedLength).ToBase64().HashHexString(IdentifierHashAlgorithm);
        }
    }
}
