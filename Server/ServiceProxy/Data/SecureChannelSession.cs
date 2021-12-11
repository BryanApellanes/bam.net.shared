using Bam.Net.Data.Repositories;
using Bam.Net.Encryption;
using Bam.Net.ServiceProxy;
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
    public class SecureChannelSession : KeyedAuditRepoData
    {
        public const string CookieName = "bam-scs-id";

        public SecureChannelSession(Instant clientNow = null, RsaKeyLength rsaKeyLength = RsaKeyLength._2048)
        {
            if(clientNow == null)
            {
                clientNow = new Instant();
            }
            IdentifierHashAlgorithm = HashAlgorithms.SHA256;
            IdentifierSeedLength = 128;
            DateTime dateTimeNow = DateTime.UtcNow;
            Created = dateTimeNow;
            LastActivity = dateTimeNow;
            TimeOffset = clientNow.DiffInMilliseconds(dateTimeNow);
            AsymmetricCipherKeyPair asymmetricKeys = Rsa.GenerateKeyPair(rsaKeyLength);
            
        }

        public SecureChannelSession(bool initializeIdentitifer) : this()
        {
            if (initializeIdentitifer)
            {
                SetIdentifier();
            }
        }

        public HashAlgorithms IdentifierHashAlgorithm { get; set; }

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

        internal int IdentifierSeedLength { get; set; }

        private void SetIdentifier()
        {
            SecureRandom secureRandom = new SecureRandom();
            Identifier = secureRandom.GenerateSeed(IdentifierSeedLength).ToBase64().HashHexString(IdentifierHashAlgorithm);
        }

        public static SecureChannelSession GetSecureChannelSessionForContext(IHttpContext httpContext, IRepositoryResolver repositoryResolver)
        {
            Args.ThrowIfNull(httpContext, nameof(httpContext));
            string secureChannelSessionId = GetSecureChannelSessionIdentifier(httpContext.Request);

            SecureChannelSession secureChannelSession;
            IRepository repository = repositoryResolver.GetRepository(httpContext);
            if (string.IsNullOrEmpty(secureChannelSessionId))
            {
                secureChannelSession = CreateSecureChannelSession(repository);
            }
            else
            {
                secureChannelSession = RetrieveSecureChannelSession(secureChannelSessionId, repository);
            }

            EnsureSecureChannelSessionIdentifierCookie(httpContext.Response, secureChannelSession.Identifier);

            return secureChannelSession;
        }

        public static string GetSecureChannelSessionIdentifier(IRequest request)
        {
            Cookie cookie = request.Cookies[CookieName];
            if(cookie != null)
            {
                return cookie.Value;
            }
            return string.Empty;
        }

        protected static SecureChannelSession CreateSecureChannelSession(IRepository repository)
        {
            SecureChannelSession secureChannelSession = new SecureChannelSession(true);
            repository.Save(secureChannelSession);
            return secureChannelSession;
        }

        protected static void EnsureSecureChannelSessionIdentifierCookie(IResponse response, string secureChannelSessionId)
        {

        }

        public static SecureChannelSession RetrieveSecureChannelSession(string sessionIdentifier, IRepository repository)
        {
            throw new NotImplementedException();
        }
    }
}
