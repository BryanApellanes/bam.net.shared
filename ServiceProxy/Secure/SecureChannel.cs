/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Bam.Net.Logging;
using Bam.Net.Incubation;
using Bam.Net.Web;
using Bam.Net.Encryption;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Engines;
using Bam.Net.Configuration;
using Bam.Net.CoreServices;

namespace Bam.Net.ServiceProxy.Secure
{
    /// <summary>
    /// A secure communication channel.  Provides 
    /// application layer encrypted communication
    /// </summary>
    [Proxy("secureChannel")]
    public partial class SecureChannel: IRequiresHttpContext, IHasServiceRegistry
    {
        public SecureChannel()
        {
            this.SecureChannelSessionManager = new SecureChannelSessionManager();
        }

        public SecureChannel(ISecureChannelSessionManager secureChannelSessionManager)
        {
            this.SecureChannelSessionManager = secureChannelSessionManager;
        }

        [Exclude]
        public object Clone()
        {
            SecureChannel clone = new SecureChannel();
            clone.CopyProperties(this);
            
            return clone;
        }

        ILogger _logger;
        object _loggerSync = new object();
        public ILogger Logger
        {
            get
            {
                return _loggerSync.DoubleCheckLock(ref _logger, () => Log.Default);
            }
            set
            {
                _logger = value;
            }
        }

        protected ISecureChannelSessionManager SecureChannelSessionManager { get; set; }


        /// <summary>
        /// Establish a secure session
        /// </summary>
        /// <returns></returns>
        public SecureChannelMessage<ClientSessionInfo> StartSession(Instant instant)
        {
            SecureSession session = SecureSession.Get(HttpContext, instant);
            ClientSessionInfo result = GetClientSessionInfo(session);

            SetSessionCookie(session);

            return new SecureChannelMessage<ClientSessionInfo>(result);
        }

        public void EndSession(string sessionIdentifier)
        {
            SecureSession session = SecureSession.Get(sessionIdentifier);
            session.Delete();
            Log.AddEntry("EndSession: Session {0} was deleted", sessionIdentifier);
        }

        private void SetSessionCookie(SecureSession session)
        {
            Cookie sessionCookie = HttpContext.Response.Cookies[SecureSession.CookieName];
            if (sessionCookie == null)
            {
                HttpContext.Response.Cookies.Add(new Cookie(SecureSession.CookieName, session.Identifier));
            }
        }

        internal static ClientSessionInfo GetClientSessionInfo(SecureSession session)
        {
            ClientSessionInfo result = new ClientSessionInfo()
            {
                SessionId = session.Id.Value,
                ClientIdentifier = session.Identifier,
                PublicKey = session.PublicKey
            };
            return result;
        }

        public SecureChannelMessage SetSessionKey(SetSessionKeyRequest request)
        {
            SecureChannelMessage result = new SecureChannelMessage(true);
            try
            {
                SecureSession session = SecureSession.Get(HttpContext);
                session.SetSymmetricKey(request);
            }
            catch (Exception ex)
            {
                result = new SecureChannelMessage(ex);
            }

            return result;
        }

        IApiKeyResolver _apiKeyResolver;
        object _apiKeyResolverSync = new object();
        public IApiKeyResolver ApiKeyResolver
        {
            get
            {
                return _apiKeyResolverSync.DoubleCheckLock(ref _apiKeyResolver, () => new ApiKeyResolver());
            }
            set
            {
                _apiKeyResolver = value;
            }
        }

        public static bool Debug
        {
            get;
            set;
        }

        static ServiceRegistry _serviceRegistry;
        static object _serviceRegistrySync = new object();
        /// <summary>
        /// The incubator used for SecureChannel requests
        /// </summary>
        public ServiceRegistry ServiceRegistry
        {
            get
            {
                return _serviceRegistrySync.DoubleCheckLock(ref _serviceRegistry, () =>
                {
                    return new ServiceRegistry();                    
                });
            }
            set
            {
                _serviceRegistry = value;
            }
        }

        public IHttpContext HttpContext
        {
            get;
            set;
        }

    }
}
