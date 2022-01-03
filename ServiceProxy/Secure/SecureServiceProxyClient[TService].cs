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

        ISecureChannelSessionDataManager _secureChannelSessionManager;
        object _secureChannelSessionManagerLock = new object();
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

        IApiKeyResolver _apiKeyResolver;
        object _apiKeyResolverSync = new object();
        [Inject]
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
        [Inject]
        public IApiEncryptionProvider ApiEncryptionProvider
        {
            get
            {
                return _apiEncryptionProviderLock.DoubleCheckLock(ref _apiEncryptionProvider, () => new ApiEncryptionProvider(SecureChannelSessionDataManager));
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
                return ClientSession != null && !string.IsNullOrEmpty(ClientSession.PublicKey);
            }            
        }

        ClientSession _sessionInfo;
        public ClientSession ClientSession
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

        public async Task<ClientSession> StartClientSessionAsync(Instant clientNow)
        {
            OnSessionStarting();

            try
            {
                ServiceProxyInvocationRequest<SecureChannel> serviceProxyInvocationRequest = new ServiceProxyInvocationRequest<SecureChannel>(nameof(SecureChannel.StartSession), clientNow);
                HttpRequestMessage requestMessage = await CreateServiceProxyRequestMessageAsync(serviceProxyInvocationRequest);
                HttpResponseMessage responseMessage = await HttpClient.SendAsync(requestMessage);
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                LastResponse = responseString;
                responseMessage.EnsureSuccessStatusCode();
                SecureChannelResponseMessage<ClientSession> secureChannelMessage = responseString.FromJson<SecureChannelResponseMessage<ClientSession>>();
                if (!secureChannelMessage.Success)
                {
                    throw new SecureChannelException(secureChannelMessage);
                }

                ClientSession = secureChannelMessage.Data;
                ClientSession.RemoteHostName = BaseAddress;
                await SetSessionKeyAsync();
                OnSessionStarted();
                return ClientSession;
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
            SetSessionKeyRequest setSessionKeyRequest = ClientSession.CreateSetSessionKeyRequest();
            ServiceProxyInvocationRequest<SecureChannel> serviceProxyInvocationRequest = new ServiceProxyInvocationRequest<SecureChannel>(nameof(SecureChannel.SetSessionKey), setSessionKeyRequest);
            HttpRequestMessage requestMessage = await CreateServiceProxyRequestMessageAsync(serviceProxyInvocationRequest);
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
                    Decrypted decrypted = new Decrypted(result.Data, ClientSession.AesKey, ClientSession.AesIV);
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
                    HttpRequestMessage requestMessage = await CreateServiceProxyRequestMessageAsync(serviceProxyInvocationRequest);
                    SecureServiceProxyInvocationRequestArguments<TService> secureServiceProxyArguments = new SecureServiceProxyInvocationRequestArguments<TService>(ClientSession, ApiKeyResolver, ApiEncryptionProvider, serviceProxyInvocationRequest);
                    secureServiceProxyArguments.WriteArgumentContent(requestMessage);
                    secureServiceProxyArguments.SetKeyToken(requestMessage);

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

        private void Initialize()
        {
            this.InvokeMethodStarted += (s, a) => _startSessionTask = TryStartSessionAsync();
        }

        Task<ClientSession> _startSessionTask;
        private async Task<ClientSession> TryStartSessionAsync()
        {
            try
            {
                if (ClientSession == null)
                {
                    if (_startSessionTask == null)
                    {
                        _startSessionTask = StartClientSessionAsync(new Instant());
                    }
                    ClientSession = await _startSessionTask;
                }
            }
            catch (Exception ex)
            {
                SessionStartException = ex;
            }
            return ClientSession;
        }
    }
}
