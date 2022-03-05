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
using Bam.Net.ServiceProxy.Data;

namespace Bam.Net.ServiceProxy.Encryption
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
            this.SecureChannelSessionDataManager = new SecureChannelSessionDataManager();
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

        ISecureChannelSessionDataManager _secureChannelSessionManager;
        protected ISecureChannelSessionDataManager SecureChannelSessionDataManager 
        {
            get
            {
                if(_secureChannelSessionManager == null)
                {
                    _secureChannelSessionManager = ServiceRegistry.Get<ISecureChannelSessionDataManager>();
                }
                return _secureChannelSessionManager;
            }
            set
            {
                _secureChannelSessionManager = value;
            }
        }

        public async Task<ClientSessionInfo> StartSessionAsync()
        {
            return await Task.Run(() => StartSession(new Instant()).Data);
        }

        /// <summary>
        /// Establish a secure channel session.
        /// </summary>
        /// <returns></returns>
        public SecureChannelResponseMessage<ClientSessionInfo> StartSession(Instant instant)
        {
            SecureChannelSession secureChannelSession = SecureChannelSessionDataManager.GetSecureChannelSessionForContextAsync(HttpContext, instant).Result;
            ClientSessionInfo clientSessionInfo = secureChannelSession.GetClientSession(false);

            return new SecureChannelResponseMessage<ClientSessionInfo>(clientSessionInfo);
        }

        public void EndSession(string sessionIdentifier)
        {
            SecureSession session = SecureSession.Get(sessionIdentifier);
            session.Delete();
            Log.AddEntry("EndSession: Session {0} was deleted", sessionIdentifier);
        }

        public SecureChannelResponseMessage SetSessionKey(SetSessionKeyRequest setSessionKeyRequest)
        {
            SecureChannelResponseMessage response = new SecureChannelResponseMessage(true);
            try
            {
                SecureChannelSessionDataManager.SetSessionKeyAsync(HttpContext, setSessionKeyRequest);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new SecureChannelResponseMessage(ex);
            }

            return response;
        }


        IApiHmacKeyResolver _apiKeyResolver;
        object _apiKeyResolverSync = new object();
        public IApiHmacKeyResolver ApiKeyResolver
        {
            get
            {
                return _apiKeyResolverSync.DoubleCheckLock(ref _apiKeyResolver, () => new ApiSigningKeyResolver());
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
        /// The `ServiceRegistry` used for SecureChannel requests
        /// </summary>
        public ServiceRegistry ServiceRegistry
        {
            get
            {
                return _serviceRegistrySync.DoubleCheckLock(ref _serviceRegistry, () => new ServiceRegistry());
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
