using Bam.Net.Data.Repositories;
using Bam.Net.Web;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Secure.Data
{
    public class SecureChannelSession : KeyedAuditRepoData
    {
        public const string CookieName = "bam-scs-id";

        public SecureChannelSession()
        {
            this.IdentifierHashAlgorithm = HashAlgorithms.SHA256;
        }

        public SecureChannelSession(bool initializeIdentitifer = false): this()
        {
            if(initializeIdentitifer)
            {
                SetIdentifier();
            }
        }

        public HashAlgorithms IdentifierHashAlgorithm { get; set; }

        public string Identifier { get; set; }

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

        private void SetIdentifier()
        {
            SecureRandom secureRandom = new SecureRandom();
            this.Identifier = secureRandom.GenerateSeed(64).ToBase64().HashHexString(this.IdentifierHashAlgorithm);
        }

        public static SecureChannelSession GetSessionForContext(IHttpContext httpContext)
        {
            Args.ThrowIfNull(httpContext, nameof(httpContext));
            Args.ThrowIfNull(httpContext.Request, nameof(httpContext.Request));
            Args.ThrowIfNull(httpContext.Response, nameof(httpContext.Response));

            IRequest request = httpContext.Request;
            IResponse response = httpContext.Response;

            SecureChannelSession secureChannelSession = null;
            Cookie cookie = request.Cookies[CookieName];
            if (cookie == null)
            {
                string secureChannelSessionIdentifier = request.Headers[Headers.SecureChannelSessionId];
                if (string.IsNullOrEmpty(secureChannelSessionIdentifier))
                {
                    secureChannelSession = new SecureChannelSession(true);
                    secureChannelSessionIdentifier = secureChannelSession.Identifier;
                    cookie = new Cookie(CookieName, secureChannelSessionIdentifier);
                    response.Cookies.Add(cookie);
                }
            }
            throw new NotImplementedException();
        }

        public static SecureChannelSession Retrieve(string sessionIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
