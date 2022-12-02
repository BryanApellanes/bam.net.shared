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
using Bam.Net.Server.ServiceProxy;
using Bam.Net.Server;
using Bam.Net.Services;

namespace Bam.Net.ServiceProxy.Encryption
{
    /// <summary>
    /// A secure communication channel.  Provides 
    /// application layer encrypted communication
    /// </summary>
    [Proxy("secureChannel")]
    public partial class SecureChannel: IRequiresHttpContext, IHasApplicationServiceRegistry, IHasWebServiceRegistry
    {
        public SecureChannel()
        { 
            this.SecureChannelSessionDataManager = new SecureChannelSessionDataManager();
        }

        [Exclude]
        public object Clone()
        {
            SecureChannel clone = new SecureChannel()
            { 
                ApplicationServiceRegistry = ApplicationServiceRegistry,
                WebServiceProxyDescriptorsProvider = WebServiceProxyDescriptorsProvider 
            };
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

        [Inject]
        public IApplicationNameProvider ApplicationNameProvider
        {
            get;
            set;
        }

        internal IWebServiceProxyDescriptorsProvider WebServiceProxyDescriptorsProvider
        {
            get;
            set;
        }

        ISecureChannelSessionDataManager _secureChannelSessionManager;
        protected ISecureChannelSessionDataManager SecureChannelSessionDataManager 
        {
            get
            {
                if(_secureChannelSessionManager == null)
                {
                    _secureChannelSessionManager = WebServiceRegistry.Get<ISecureChannelSessionDataManager>();
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
            ClientSessionInfo clientSessionInfo = GetClientSessionInfo(instant);

            return new SecureChannelResponseMessage<ClientSessionInfo>(clientSessionInfo);
        }

        public SecureChannelResponseMessage Execute(SecureChannelRequestMessage secureChannelRequestMessage)
        {
            WebServiceProxyDescriptors webServiceProxyDescriptors = WebServiceProxyDescriptorsProvider.GetWebServiceProxyDescriptors(ApplicationNameProvider.GetApplicationName());

            ServiceProxyInvocation serviceProxyInvocation = secureChannelRequestMessage.ToServiceProxyInvocation(webServiceProxyDescriptors, new InputStreamServiceProxyInvocationArgumentReader());

            if (serviceProxyInvocation.Execute())
            {
                ClientSessionInfo clientSessionInfo = GetClientSessionInfo();
                AesKeyVectorPair aesKey = clientSessionInfo.GetAesKey();
                string resultJson = serviceProxyInvocation.Result?.ToJson();
                string jsonCipher = aesKey.Encrypt(resultJson);
                return new SecureChannelResponseMessage(true)
                {
                    Data = jsonCipher
                };
            }

            return new SecureChannelResponseMessage(serviceProxyInvocation.Exception ?? new Exception($"{nameof(ServiceProxyInvocation)}.Execute() failed"));
        }

        public void EndSession(string sessionIdentifier)
        {
            _ = SecureChannelSessionDataManager.EndSecureChannelSessionAsync(sessionIdentifier);
            Log.AddEntry("EndSession: Session {0} was set to deleted", sessionIdentifier);
        }

        public SecureChannelResponseMessage SetSessionKey(SetSessionKeyRequest setSessionKeyRequest)
        {
            SecureChannelResponseMessage response = new SecureChannelResponseMessage(true);
            try
            {
                SecureChannelSessionDataManager.SetSessionKeyAsync(HttpContext, setSessionKeyRequest).Wait();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new SecureChannelResponseMessage(ex);
            }

            return response;
        }


        IApiHmacKeyResolver _apiHmacKeyResolver;
        object _apiKeyResolverSync = new object();
        public IApiHmacKeyResolver ApiHmacKeyResolver
        {
            get
            {
                return _apiKeyResolverSync.DoubleCheckLock(ref _apiHmacKeyResolver, () => new ApiHmacKeyResolver());
            }
            set
            {
                _apiHmacKeyResolver = value;
            }
        }

        static WebServiceRegistry _webServiceRegistry;
        static object _serviceRegistrySync = new object();
        /// <summary>
        /// The `ServiceRegistry` used for SecureChannel requests
        /// </summary>
        public WebServiceRegistry WebServiceRegistry
        {
            get
            {
                return _serviceRegistrySync.DoubleCheckLock(ref _webServiceRegistry, () => new WebServiceRegistry());
            }
            set
            {
                _webServiceRegistry = value;
            }
        }

        public IHttpContext HttpContext
        {
            get;
            set;
        }

        static ApplicationServiceRegistry _applicationServiceRegistry;
        static object _applicationServiceRegistrySync = new object();
        public ApplicationServiceRegistry ApplicationServiceRegistry 
        {
            get
            {
                return _applicationServiceRegistrySync.DoubleCheckLock(ref _applicationServiceRegistry, () => ApplicationServiceRegistry.Current);
            }
            set
            {
                _applicationServiceRegistry = value;
            }
        }

        private ClientSessionInfo GetClientSessionInfo(Instant instant = null)
        {
            SecureChannelSession secureChannelSession = SecureChannelSessionDataManager.GetSecureChannelSessionForContextAsync(HttpContext, instant).Result;
            ClientSessionInfo clientSessionInfo = secureChannelSession.GetClientSession(false);
            return clientSessionInfo;
        }
    }
}
