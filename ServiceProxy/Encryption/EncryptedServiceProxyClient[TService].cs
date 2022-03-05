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
using System.Web;
using Bam.Net.Services;

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
        public IApiHmacKeyResolver ApiSigningKeyResolver
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

        IApiValidationProvider _apiEncryptionProvider;
        object _apiEncryptionProviderLock = new object();
        [Inject]
        public IApiValidationProvider ApiValidationProvider
        {
            get
            {
                return _apiEncryptionProviderLock.DoubleCheckLock(ref _apiEncryptionProvider, () => new ApiValidationProvider(SecureChannelSessionDataManager));
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
        public event Action<EncryptedServiceProxyClient<TService>, Exception> StartSessionException;
        protected void OnStartSessionException(Exception ex)
        {
            StartSessionException?.Invoke(this, ex);
        }

        public async Task<ClientSessionInfo> StartClientSessionAsync(Instant clientNow)
        {
            OnSessionStarting();

            try
            {
                ServiceProxyInvocationRequest<SecureChannel> serviceProxyInvocationRequest = new ServiceProxyInvocationRequest<SecureChannel>(nameof(SecureChannel.StartSession), clientNow) { BaseAddress = this.BaseAddress };
                HttpRequestMessage requestMessage = await base.CreateServiceProxyInvocationRequestMessageAsync(serviceProxyInvocationRequest);
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
                SessionStartException = ex;
                OnStartSessionException(ex);
            }

            return null;
        }

        protected async Task SetSessionKeyAsync()
        {
            SetSessionKeyRequest setSessionKeyRequest = ClientSessionInfo.CreateSetSessionKeyRequest();
            ServiceProxyInvocationRequest<SecureChannel> serviceProxyInvocationRequest = new ServiceProxyInvocationRequest<SecureChannel>(nameof(SecureChannel.SetSessionKey), setSessionKeyRequest) { BaseAddress = this.BaseAddress };
            HttpRequestMessage requestMessage = await base.CreateServiceProxyInvocationRequestMessageAsync(serviceProxyInvocationRequest);
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
                    //EncryptedServiceProxyInvocationRequestArgumentWriter<TService> secureServiceProxyArguments = new EncryptedServiceProxyInvocationRequestArgumentWriter<TService>(ClientSessionInfo, ApiSigningKeyResolver, ApiValidationProvider, serviceProxyInvocationRequest);
                    //secureServiceProxyArguments.WriteArgumentContent(requestMessage);
                    //secureServiceProxyArguments.SetKeyToken(requestMessage);

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

            EncryptedServiceProxyInvocationRequestWriter requestWriter = GetRequestWriter<EncryptedServiceProxyInvocationRequestWriter>(ClientSessionInfo, ApiSigningKeyResolver, ApiValidationProvider);
            HttpRequestMessage httpRequestMessage = await requestWriter.WriteRequestMessageAsync(serviceProxyInvocationRequest);

            Headers.Keys.Each(key => httpRequestMessage.Headers.Add(key, Headers[key]));
            return httpRequestMessage;
        }

        private void Initialize()
        {
            this.InvokeMethodStarted += (s, a) => _startSessionTask = TryStartSessionAsync();
        }

        Task<ClientSessionInfo> _startSessionTask;
        private async Task<ClientSessionInfo> TryStartSessionAsync()
        {
            try
            {
                if (ClientSessionInfo == null)
                {
                    if (_startSessionTask == null)
                    {
                        _startSessionTask = StartClientSessionAsync(new Instant());
                    }
                    ClientSessionInfo = await _startSessionTask;
                }

                if (ClientSessionInfo == null)
                {
                    throw new ArgumentNullException("Failed to start session");
                }
            }
            catch (Exception ex)
            {
                SessionStartException = ex;
            }
            
            return ClientSessionInfo;
        }
    }
}
