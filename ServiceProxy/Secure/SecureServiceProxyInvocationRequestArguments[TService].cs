using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Bam.Net.ServiceProxy.Secure
{
    public class SecureServiceProxyInvocationRequestArguments<TService> : ServiceProxyInvocationRequestArguments<TService>
    {
        public SecureServiceProxyInvocationRequestArguments(ClientSessionInfo clientSessionInfo, IApiKeyResolver apiKeyResolver, IApiEncryptionProvider apiEncryptionProvider, ServiceProxyInvocationRequest serviceProxyInvokeRequest) : base(serviceProxyInvokeRequest)
        {
            this.ApiKeyResolver = apiKeyResolver;
            this.ApiEncryptionProvider = apiEncryptionProvider;
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

        public void SetKeyToken(HttpRequestMessage requestMessage)
        {
            if(TypeRequiresApiKey || MethodRequiresApiKey)
            {
                ApiKeyResolver.SetKeyToken(requestMessage, ApiArgumentEncoder.GetStringToHash(typeof(TService).Name, MethodInfo.Name, GetJsonArgsMember()));
            }            
        }

        /// <summary>
        /// Sets the content of the specified request message to the cipher of the invocation request.  Additionally, applies encrypted validation token headers.
        /// </summary>
        /// <param name="requestMessage"></param>
        public override void WriteArgumentContent(HttpRequestMessage requestMessage)
        {
            SecureChannelRequestMessage secureChannelRequestMessage = new SecureChannelRequestMessage(ServiceProxyInvocationRequest);

            requestMessage.Content = secureChannelRequestMessage.GetSymetricCipherContent(ClientSessionInfo);
            secureChannelRequestMessage.SetEncryptedValidationTokenHeaders(ApiEncryptionProvider, ClientSessionInfo, requestMessage);
        }
    }
}
