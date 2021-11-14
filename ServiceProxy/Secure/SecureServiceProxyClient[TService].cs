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
        public SecureServiceProxyClient(string baseAddress)
            : base(baseAddress)
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

        /// <summary>
        /// The key for the current session.
        /// </summary>
        protected internal string SessionKey
        {
            get
            {
                return SessionInfo?.SessionKey;
            }
            set
            {
                Args.ThrowIf<ArgumentNullException>(SessionInfo == null, "SessionInfo not set");
                SessionInfo.SessionKey = value;
            }
        }

        /// <summary>
        /// The initialization vector for the current session
        /// </summary>
        protected internal string SessionIV
        {
            get
            {
                return SessionInfo?.SessionIV;
            }
            set
            {
                Args.ThrowIf<ArgumentNullException>(SessionInfo == null, "SessionInfo not set");
                SessionInfo.SessionIV = value;
            }
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

        public async Task<ClientSessionInfo> StartSessionAsync()
        {
            OnSessionStarting();

            try
            {
                HttpRequestMessage requestMessage = CreateServiceProxyRequestMessageAsync(ServiceProxyVerbs.Post, nameof(SecureChannel), nameof(SecureChannel.StartSession), new Instant()).Result;
                HttpResponseMessage responseMessage = await HttpClient.SendAsync(requestMessage);
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                LastResponse = responseString;
                responseMessage.EnsureSuccessStatusCode();
                await SetSessionKeyAsync();
                OnSessionStarted();
                return responseString.FromJson<ClientSessionInfo>();
            }
            catch (Exception ex)
            {
                SessionStartException = ex;
                OnStartSessionException(ex);
            }

            return null;
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

        public override async Task<string> ReceivePostResponseAsync(ServiceProxyInvokeRequest serviceProxyInvokeRequest)
        {
            ServiceProxyInvokeEventArgs args = new ServiceProxyInvokeEventArgs(serviceProxyInvokeRequest);
            args.Client = this;
            OnPosting(args);
            string response = string.Empty;
            if (args.CancelInvoke)
            {
                OnPostCanceled(args);
            }
            else
            {
                try
                {
                    HttpRequestMessage requestMessage = await CreateServiceProxyRequestMessageAsync(ServiceProxyVerbs.Post, nameof(SecureChannel), nameof(SecureChannel.Invoke), string.Empty);

                    SecureServiceProxyArguments<TService> secureServiceProxyArguments = new SecureServiceProxyArguments<TService>(SessionInfo, ApiKeyResolver, ApiEncryptionProvider, serviceProxyInvokeRequest);
                    secureServiceProxyArguments.SetContent(requestMessage);
                    secureServiceProxyArguments.SetKeyToken(requestMessage, serviceProxyInvokeRequest.MethodName);

                    HttpResponseMessage responseMessage = await HttpClient.SendAsync(requestMessage);
                    args.RequestMessage = requestMessage;
                    args.ResponseMessage = responseMessage;
                    response = await responseMessage.Content.ReadAsStringAsync();
                    responseMessage.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    args.Exception = ex;
                    OnRequestExceptionThrown(args);
                }
            }
            return response;
        }

        protected async Task SetSessionKeyAsync()
        {
            SetSessionKeyRequest request = CreateSetSessionKeyRequest(out AesKeyVectorPair kvp);           

            SecureChannelMessage response = await this.ReceivePostResponseAsync<SecureChannelMessage>(nameof(SecureChannel), nameof(SecureChannel.SetSessionKey), new object[] { request });
            if (!response.Success)
            {
                throw new Exception(response.Message);
            }

            SessionKey = kvp.Key;
            SessionIV = kvp.IV;
        }

        protected internal SetSessionKeyRequest CreateSetSessionKeyRequest(out AesKeyVectorPair kvp)
        {
            return SecureSession.CreateSetSessionKeyRequest(SessionInfo.PublicKey, out kvp);
        }

        private void Initialize()
        {
            this.InvokeMethodStarted += async (s, a) => await TryStartSessionAsync();
        }

        Task<ClientSessionInfo> _startSessionTask;
        private async Task TryStartSessionAsync()
        {
            try
            {
                if (SessionInfo == null)
                {
                    if (_startSessionTask == null)
                    {
                        _startSessionTask = StartSessionAsync();
                    }
                    SessionInfo = await _startSessionTask;
                }
            }
            catch (Exception ex)
            {
                SessionStartException = ex;
            }
        }
    }
}
