/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net.Encryption;
using Bam.Net.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Encryption
{
    /// <summary>
    /// A service proxy client that uses application level encryption.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class EncryptedServiceProxyClient<TService>: ServiceProxyClient<TService>
    {
        public EncryptedServiceProxyClient(string baseAddress)
            : base(baseAddress)
        {
            this.Initialize();
        }

        ISecureChannelSessionDataManager _secureChannelSessionManager;
        readonly object _secureChannelSessionManagerLock = new object();
        [Inject]
        public ISecureChannelSessionDataManager SecureChannelSessionDataManager 
        {
            get
            {
                return _secureChannelSessionManagerLock.DoubleCheckLock(ref _secureChannelSessionManager, () => new SecureChannelSessionDataManager());
            }
            set
            {
                _secureChannelSessionManager = value;
            }
        }

        IApiHmacKeyResolver _apiKeyResolver;
        object _apiKeyResolverSync = new object();
        [Inject]
        public IApiHmacKeyResolver ApiHmacKeyResolver
        {
            get
            {
                return _apiKeyResolverSync.DoubleCheckLock(ref _apiKeyResolver, () => new ApiHmacKeyResolver());
            }
            set
            {
                _apiKeyResolver = value;
            }
        }

        /// <summary>
        /// Gets the exception that occurred during session start.  May be null if session started successfully.
        /// </summary>
        public Exception StartSessionException
        {
            get;
            private set;
        }

        public bool IsSessionStarted
        {
            get
            {
                return ClientSessionInfo != null && !string.IsNullOrEmpty(ClientSessionInfo.PublicKey);
            }            
        }

        ClientSessionInfo _sessionInfo;
        public ClientSessionInfo ClientSessionInfo
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

        public event Action<EncryptedServiceProxyClient<TService>> SessionStarting;
        protected void OnSessionStarting()
        {
            SessionStarting?.Invoke(this);
        }

        public event Action<EncryptedServiceProxyClient<TService>> SessionStarted;
        protected void OnSessionStarted()
        {
            SessionStarted?.Invoke(this);
        }

        /// <summary>
        /// The event that is raised if an exception occurs starting the 
        /// secure session.
        /// </summary>
        public event Action<EncryptedServiceProxyClient<TService>, Exception> SessionStartException;
        protected void OnSessionStartException(Exception ex)
        {
            SessionStartException?.Invoke(this, ex);
        }

        public async Task<ClientSessionInfo> StartClientSessionAsync(Instant clientNow)
        {
            OnSessionStarting();

            try
            {
                ServiceProxyInvocationRequest<SecureChannel> serviceProxyInvocationRequest = new ServiceProxyInvocationRequest<SecureChannel>(nameof(SecureChannel.StartSession), clientNow) { BaseAddress = this.BaseAddress };
                ServiceProxyInvocationRequestWriter serviceProxyInvocationRequestWriter = new ServiceProxyInvocationRequestWriter();
                HttpRequestMessage requestMessage = await serviceProxyInvocationRequestWriter.WriteRequestMessageAsync(serviceProxyInvocationRequest);
                HttpResponseMessage responseMessage = await HttpClient.SendAsync(requestMessage);
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                LastResponse = responseString;
                responseMessage.EnsureSuccessStatusCode();
                SecureChannelResponseMessage<ClientSessionInfo> secureChannelMessage = responseString.FromJson<SecureChannelResponseMessage<ClientSessionInfo>>();
                if (!secureChannelMessage.Success)
                {
                    throw new SecureChannelException(secureChannelMessage);
                }

                ClientSessionInfo = secureChannelMessage.Data;
                ClientSessionInfo.ServerHostName = BaseAddress;
                await SetSessionKeyAsync();
                OnSessionStarted();
                return ClientSessionInfo;
            }
            catch (Exception ex)
            {
                StartSessionException = ex;
                OnSessionStartException(ex);
            }

            return null;
        }

        protected async Task SetSessionKeyAsync()
        {
            SetSessionKeyRequest setSessionKeyRequest = ClientSessionInfo.CreateSetSessionKeyRequest();
            ServiceProxyInvocationRequest<SecureChannel> serviceProxyInvocationRequest = new ServiceProxyInvocationRequest<SecureChannel>(nameof(SecureChannel.SetSessionKey), setSessionKeyRequest) { BaseAddress = this.BaseAddress };
            ServiceProxyInvocationRequestWriter serviceProxyInvocationRequestWriter = new ServiceProxyInvocationRequestWriter();
            HttpRequestMessage requestMessage = await serviceProxyInvocationRequestWriter.WriteRequestMessageAsync(serviceProxyInvocationRequest);
            HttpResponseMessage responseMessage = await HttpClient.SendAsync(requestMessage);
            string responseString = await responseMessage.Content.ReadAsStringAsync();
            LastResponse = responseString;
            responseMessage.EnsureSuccessStatusCode();
            SecureChannelResponseMessage response = responseString.FromJson<SecureChannelResponseMessage>();
            if (!response.Success)
            {
                throw new Exception(response.Message);
            }
        }

        protected internal override async Task<string> ReceiveServiceMethodResponseAsync(ServiceProxyInvocationRequest<TService> request)
        {
            try
            {
                _startSessionTask.Wait();
                string responseString = await ReceivePostResponseAsync(request);
                SecureChannelResponseMessage<string> result = responseString.FromJson<SecureChannelResponseMessage<string>>();
                if (result.Success)
                {
                    Decrypted decrypted = new Decrypted(result.Data, ClientSessionInfo.AesKey, ClientSessionInfo.AesIV);
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
                ServiceProxyInvocationRequestEventArgs<TService> args = new ServiceProxyInvocationRequestEventArgs<TService>(request);
                args.Exception = ex;
                args.Message = ex.Message;
                OnInvocationException(args);
            }

            return string.Empty;
        }

        public async Task<TResult> ReceivePostResponseAsync<TResult>(string className, string methodName, params object[] arguments)
        {
            return (await ReceivePostResponseAsync(new ServiceProxyInvocationRequest(this, className,  methodName, arguments))).FromJson<TResult>();
        }

        public override async Task<string> ReceivePostResponseAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            ServiceProxyInvocationRequestEventArgs args = new ServiceProxyInvocationRequestEventArgs(serviceProxyInvocationRequest);
            args.Client = this;
            OnPostStarted(args);
            string response = string.Empty;
            if (args.CancelInvoke)
            {
                OnPostCanceled(args);
            }
            else
            {
                try
                {
                    EncryptedServiceProxyInvocationRequest secureServiceProxyInvocationRequest = serviceProxyInvocationRequest as EncryptedServiceProxyInvocationRequest; 
                    if(secureServiceProxyInvocationRequest == null)
                    {
                        secureServiceProxyInvocationRequest = serviceProxyInvocationRequest.CopyAs<EncryptedServiceProxyInvocationRequest>();
                    }
                    HttpRequestMessage requestMessage = await CreateServiceProxyInvocationRequestMessageAsync(secureServiceProxyInvocationRequest);

                    HttpResponseMessage responseMessage = await HttpClient.SendAsync(requestMessage);
                    args.RequestMessage = requestMessage;
                    args.ResponseMessage = responseMessage;
                    response = await responseMessage.Content.ReadAsStringAsync();
                    responseMessage.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    args.Exception = ex;
                    OnInvocationException(args);
                }
            }
            return response;
        }

        public virtual async Task<HttpRequestMessage> CreateServiceProxyInvocationRequestMessageAsync(EncryptedServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            if (serviceProxyInvocationRequest == null)
            {
                throw new ArgumentNullException(nameof(serviceProxyInvocationRequest));
            }

            if (serviceProxyInvocationRequest.ServiceType == null)
            {
                throw new ArgumentNullException("ServiceType not specified");
            }

            IServiceProxyInvocationRequestWriter requestWriter = GetRequestWriter();
            HttpRequestMessage httpRequestMessage = await requestWriter.WriteRequestMessageAsync(serviceProxyInvocationRequest);

            Headers.Keys.Each(key => httpRequestMessage.Headers.Add(key, Headers[key]));
            return httpRequestMessage;
        }

        protected override IServiceProxyInvocationRequestWriter GetRequestWriter()
        {
            return new EncryptedServiceProxyInvocationRequestWriter(ClientSessionInfo, ApiHmacKeyResolver);
        }

        private void Initialize()
        {
            this.InvocationStarted += (s, a) => _startSessionTask = TryStartSessionAsync();
        }

        Task<ClientSessionInfo> _startSessionTask;
        object _startSessionLock = new object();
        private async Task<ClientSessionInfo> TryStartSessionAsync()
        {
            try
            {
                if (ClientSessionInfo == null)
                {
                    lock (_startSessionLock)
                    {
                        if (_startSessionTask == null)
                        {
                            _startSessionTask = StartClientSessionAsync(new Instant());
                        }
                    }                    
                }

                ClientSessionInfo = await _startSessionTask;
                if (ClientSessionInfo == null)
                {
                    throw new ArgumentNullException("Failed to start session");
                }
            }
            catch (Exception ex)
            {
                StartSessionException = ex;
            }
            
            return ClientSessionInfo;
        }
    }
}
