/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Bam.Net;
using Bam.Net.Encryption;
using Bam.Net.Configuration;
using Bam.Net.ServiceProxy;
using Bam.Net.Logging;
using Bam.Net.Web;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System.IO;
using System.Reflection;
using System.Net.Http;

namespace Bam.Net.ServiceProxy.Secure
{
    /// <summary>
    /// A secure service proxy client that uses application level encryption
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class SecureServiceProxyClient<TService>: ServiceProxyClient<TService>
    {
        public const string AsymetricCipherMediaType = "application/vnd.bam+cipher;algorithm=asymetric";
        public const string SymetricCipherMediaType = "application/vnd.bam+cipher;algorithm=symetric";

        public SecureServiceProxyClient(string baseAddress)
            : base(baseAddress)
        {
            this.Initialize();
        }
        
        public SecureServiceProxyClient(string baseAddress, string implementingClassName)
            : base(baseAddress, implementingClassName)
        {
            this.Initialize();
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

        IApiEncryptionProvider _apiEncryptionProvider;
        object _apiEncryptionProviderLock = new object();
        public IApiEncryptionProvider ApiEncryptionProvider
        {
            get
            {
                return _apiEncryptionProviderLock.DoubleCheckLock(ref _apiEncryptionProvider, () => new DefaultApiEncryptionProvider());
            }
            set
            {
                _apiEncryptionProvider = value;
            }
        }

        public Exception SessionStartException
        {
            get;
            private set;
        }

        public bool IsSessionEstablished
        {
            get
            {
                return SessionInfo != null && SessionInfo.SessionId > 0 && !string.IsNullOrEmpty(SessionInfo.PublicKey);
            }            
        }

        [Obsolete("Use SecureSessionId instead")]
        public Cookie SecureSessionCookie
        {
            get;
            protected internal set;
        }

        public string SecureSessionId
        {
            get;
            protected internal set;
        }

        ClientSessionInfo _sessionInfo;
        public ClientSessionInfo SessionInfo
        {
            get
            {
                return _sessionInfo;
            }
            set
            {
                _sessionInfo = value;
            }
        }
        Type _type;
        /// <summary>
        /// The proxied type
        /// </summary>
        protected Type Type
        {
            get
            {
                if (_type == null)
                {
                    _type = GetType();
                    if (IsSecureServiceProxyClient)
                    {
                        _type = _type.GetGenericArguments().Single();
                    }
                }
                return _type;
            }
        }

        /// <summary>
        /// Gets a value indicating if the current instance is
        /// a SecureServiceProxyClient and not an
        /// inheriting class instance.
        /// </summary>
        protected bool IsSecureServiceProxyClient
        {
            get
            {
                if (Type.Name.Equals("SecureServiceProxyClient`1")) 
                {
                    return true;
                }
                return false;
            }
        }

        protected internal bool TypeRequiresApiKey
        {
            get
            {                
                return Type.HasCustomAttributeOfType<ApiKeyRequiredAttribute>();
            }
        }

        protected internal bool MethodRequiresApiKey(string methodName)
        {
            MethodInfo method = Type.GetMethod(methodName);
            if(method == null)
            {
                return false;
            }
            return method.HasCustomAttributeOfType<ApiKeyRequiredAttribute>();
        }

        /// <summary>
        /// The key for the current session.
        /// </summary>
        protected internal string SessionKey
        {
            get;
            set;
        }

        /// <summary>
        /// The initialization vector for the current session
        /// </summary>
        protected internal string SessionIV
        {
            get;
            set;
        }

        public event Action<SecureServiceProxyClient<TService>> SessionStarting;
        protected void OnSessionStarting()
        {
            SessionStarting?.Invoke(this);
        }

        public event Action<SecureServiceProxyClient<TService>> SessionStarted;
        protected void OnSessionStarted()
        {
            SessionStarted?.Invoke(this);
        }

        /// <summary>
        /// The event that is raised if an exception occurs starting the 
        /// secure session.
        /// </summary>
        public event Action<SecureServiceProxyClient<TService>, Exception> StartSessionException;
        protected void OnStartSessionException(Exception ex)
        {
            StartSessionException?.Invoke(this, ex);
        }

        object _sessionInfoLock = new object();
        public void StartSession()
        {
            if (SessionInfo == null)
            {
                lock (_sessionInfoLock)
                {
                    if (SessionInfo == null)
                    {
                        OnSessionStarting();

                        try
                        {
                            ServiceProxyInvokeRequest<SecureChannel> invokeRequest = new ServiceProxyInvokeRequest<SecureChannel>()
                                {
                                    MethodName = nameof(SecureChannel.InitSession)
                                };


                           
                       /*     HttpWebRequest request = GetServiceProxyRequestMessage(ServiceProxyVerbs.GET, typeof(SecureChannel).Name, "InitSession", new Instant());

                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                SessionCookie = response.Cookies[SecureSession.CookieName];
                                Cookies.Add(SessionCookie);

                                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                                {
                                    SecureChannelMessage<ClientSessionInfo> message = sr.ReadToEnd().FromJson<SecureChannelMessage<ClientSessionInfo>>();
                                    if (!message.Success)
                                    {
                                        throw new Exception(message.Message);
                                    }
                                    else
                                    {
                                        SessionInfo = message.Data;
                                    }
                                }

                                SetSessionKey();
                            }*/
                        }
                        catch (Exception ex)
                        {
                            SessionStartException = ex;
                            OnStartSessionException(ex);
                            return;
                        }

                        OnSessionStarted();
                    }
                }

            }
        }

        protected internal override async Task<string> ReceiveServiceMethodResponseAsync(ServiceProxyInvokeRequest<TService> request)
        {
            try
            {                   
                SecureChannelMessage<string> result = (await ReceivePostResponseAsync(request)).FromJson<SecureChannelMessage<string>>();
                if (result.Success)
                {
                    Decrypted decrypted = new Decrypted(result.Data, SessionKey, SessionIV);
                    return decrypted.Value;
                }
                else
                {
                    string properties = result.TryPropertiesToString();
                    throw new ServiceProxyInvocationFailedException($"{result.Message}:\r\n\t{properties}");
                }
            }
            catch (Exception ex)
            {
                ServiceProxyInvokeEventArgs<TService> args = request.TryCopyAs<ServiceProxyInvokeEventArgs<TService>>();
                args.Exception = ex;
                args.Message = ex.Message;
                OnInvocationException(args);
            }

            return string.Empty;
        }

        public async Task<TResult> ReceivePostResponseAsync<TResult>(string className, string methodName, params object[] arguments)
        {
            return (await ReceivePostResponseAsync(new ServiceProxyInvokeRequest { BaseAddress = BaseAddress, ClassName = className, MethodName = methodName, Arguments = arguments })).FromJson<TResult>();
        }

        protected override async Task<string> ReceivePostResponseAsync(ServiceProxyInvokeRequest invokeRequest, HttpRequestMessage request)
        {
            string className = invokeRequest.ClassName;
            string methodName = invokeRequest.MethodName;
            object[] arguments = invokeRequest.Arguments;
            if(className.Equals(nameof(SecureChannel), StringComparison.InvariantCultureIgnoreCase) && methodName.Equals(nameof(SecureChannel.Invoke), StringComparison.InvariantCultureIgnoreCase))
            {
                // the target is the SecureChannel.Invoke method but we
                // need the actual className and method that is in the arguments 
                // object
                string actualClassName = (string)arguments[0];
                string actualMethodName = (string)arguments[1];
                string jsonArgs = (string)arguments[2];
                HttpArgs args = new HttpArgs();
                args.ParseJson(jsonArgs);
                if(TypeRequiresApiKey || MethodRequiresApiKey(actualMethodName))
                {
                    ApiKeyResolver.SetKeyToken(request, ApiArgumentProvider.GetStringToHash(actualClassName, actualMethodName, args[JsonArgsMemberName]));
                }
            }
            return await base.ReceivePostResponseAsync(invokeRequest, request);
        }

        protected internal override void SetHttpArgsContent(string jsonArgumentsString, HttpRequestMessage request)
        {
            if (string.IsNullOrEmpty(SessionKey))
            {
                base.SetHttpArgsContent(jsonArgumentsString, request);
            }
            else
            {
                Encrypted cipher = new Encrypted(jsonArgumentsString, SessionKey, SessionIV);
                string bodyCipher = cipher.Base64Cipher;
                request.Content = new StringContent(bodyCipher, Encoding.UTF8, AsymetricCipherMediaType);

                ApiEncryptionProvider.SetEncryptedValidationToken(request, jsonArgumentsString, SessionInfo.PublicKey);
            }
        }

        protected internal override Task<HttpRequestMessage> CreateServiceProxyRequestMessageAsync(ServiceProxyVerbs verb, string className, string methodName, string queryStringParameters = "")
        {
            throw new NotImplementedException();
            // create a secure execution request
            // - execution target is SecureChannel
            // - encrypt the actual request
        }

        protected async Task SetSessionKeyAsync()
        {
            CreateSetSessionKeyRequest(out AesKeyVectorPair kvp, out SetSessionKeyRequest request);           

            SecureChannelMessage response = await this.ReceivePostResponseAsync<SecureChannelMessage>(nameof(SecureChannel), "SetSessionKey", new object[] { request });
            if (!response.Success)
            {
                throw new Exception(response.Message);
            }

            SessionKey = kvp.Key;
            SessionIV = kvp.IV;
        }

        protected internal void CreateSetSessionKeyRequest(out AesKeyVectorPair kvp, out SetSessionKeyRequest request)
        {
            request = SecureSession.CreateSetSessionKeyRequest(SessionInfo.PublicKey, out kvp);
        }

        private void Initialize()
        {
            this.InvokeMethodStarted += (s, a) => TryStartSession();
        }

        private void TryStartSession()
        {
            try
            {
                StartSession();
            }
            catch (Exception ex)
            {
                SessionStartException = ex;
            }
        }
    }
}
