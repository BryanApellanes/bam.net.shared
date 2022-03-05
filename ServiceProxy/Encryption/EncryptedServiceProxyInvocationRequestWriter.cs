using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class EncryptedServiceProxyInvocationRequestWriter : ServiceProxyInvocationRequestWriter
    {
        public EncryptedServiceProxyInvocationRequestWriter(ClientSessionInfo clientSessionInfo, IApiHmacKeyResolver apiSigningKeyResolver, IApiValidationProvider apiValidationProvider)
        {
            this.ApiHmacKeyResolver = apiSigningKeyResolver;
            this.ApiValidationProvider = apiValidationProvider;
            this.ClientSessionInfo = clientSessionInfo;

        }

        public IApiHmacKeyResolver ApiHmacKeyResolver { get; set; }
        public IApiValidationProvider ApiValidationProvider { get; set; }

        public ClientSessionInfo ClientSessionInfo { get; set; }

        public override Task<HttpRequestMessage> WriteRequestMessageAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            EncryptedServiceProxyInvocationRequest encryptedServiceProxyInvocationRequest = (serviceProxyInvocationRequest as EncryptedServiceProxyInvocationRequest) ?? serviceProxyInvocationRequest.CopyAs<EncryptedServiceProxyInvocationRequest>();
            return WriteRequestMessageAsync(encryptedServiceProxyInvocationRequest);
        }

        public virtual async Task<HttpRequestMessage> WriteRequestMessageAsync(EncryptedServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            HttpRequestMessage httpRequestMessage = await CreateServiceProxyInvocationRequestMessageAsync(serviceProxyInvocationRequest);
            EncryptedServiceProxyInvocationRequestArgumentWriter argumentWriter = serviceProxyInvocationRequest.ServiceProxyInvocationRequestArgumentWriter as EncryptedServiceProxyInvocationRequestArgumentWriter;

            Args.ThrowIfNull(argumentWriter, nameof(serviceProxyInvocationRequest.ServiceProxyInvocationRequestArgumentWriter));

            argumentWriter.ClientSessionInfo = this.ClientSessionInfo;
            
            SecureChannelRequestMessage secureChannelRequestMessage = new SecureChannelRequestMessage(serviceProxyInvocationRequest);
            
            EncryptedServiceProxyInvocationHttpRequestContext descriptor = argumentWriter.WriteEncryptedArgumentContent(httpRequestMessage, secureChannelRequestMessage);
            //ApiSigningKeyResolver
            //ApiSigningKeyResolver.SetSigningKeyToken(descriptor.HttpRequestMessage, descriptor.)
            return httpRequestMessage;
        }
    }
}
