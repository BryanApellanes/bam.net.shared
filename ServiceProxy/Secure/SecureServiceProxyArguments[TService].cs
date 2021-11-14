using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Bam.Net.ServiceProxy.Secure
{
    public class SecureServiceProxyArguments<TService> : ServiceProxyArguments<TService>
    {
        public const string SymetricCipherMediaType = "application/vnd.bam+cipher;algorithm=symetric";

        public SecureServiceProxyArguments(ClientSessionInfo clientSessionInfo, IApiKeyResolver apiKeyResolver, IApiEncryptionProvider apiEncryptionProvider, ServiceProxyInvokeRequest serviceProxyInvokeRequest) : base(serviceProxyInvokeRequest)
        {
            this.ApiKeyResolver = apiKeyResolver;
            this.ApiEncryptionProvider = ApiEncryptionProvider;
            this.ClientSessionInfo = clientSessionInfo;
        }

        public IApiKeyResolver ApiKeyResolver { get; set; }
        public IApiEncryptionProvider ApiEncryptionProvider { get; set; }

        public ClientSessionInfo ClientSessionInfo { get; set; }

        protected internal bool TypeRequiresApiKey
        {
            get
            {
                return ServiceType.HasCustomAttributeOfType<ApiKeyRequiredAttribute>();
            }
        }

        protected internal bool MethodRequiresApiKey
        {
            get
            {
                return MethodInfo.HasCustomAttributeOfType<ApiKeyRequiredAttribute>();
            }
        }

        public void SetKeyToken(HttpRequestMessage requestMessage, string methodName)
        {
            if(TypeRequiresApiKey || MethodRequiresApiKey)
            {
                ApiKeyResolver.SetKeyToken(requestMessage, ApiArgumentProvider.GetStringToHash(typeof(TService).Name, methodName, GetJsonArgsMember()));
            }            
        }

        /// <summary>
        /// Sets the content of the specified request message to the encrypted cipher of the invocation request.  Additionally, applies encrypted validation token headers.
        /// </summary>
        /// <param name="requestMessage"></param>
        public override void SetContent(HttpRequestMessage requestMessage)
        {
            SecureChannelRequestMessage<TService> secureChannelRequestMessage = new SecureChannelRequestMessage<TService>(ServiceProxyInvokeRequest);

            requestMessage.Content = secureChannelRequestMessage.GetContent(ClientSessionInfo);
            secureChannelRequestMessage.SetEncryptedValidationTokenHeaders(ApiEncryptionProvider, ClientSessionInfo, requestMessage);
        }
    }
}
