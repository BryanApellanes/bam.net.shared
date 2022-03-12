using Bam.Net.Encryption;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class EncryptedServiceProxyInvocationRequestWriter : ServiceProxyInvocationRequestWriter
    {
        public EncryptedServiceProxyInvocationRequestWriter(ClientSessionInfo clientSessionInfo, IApiHmacKeyResolver apiHmacKeyResolver)//, IApiValidationProvider apiValidationProvider)
        {
            this.ApiHmacKeyResolver = apiHmacKeyResolver;
            this.ClientSessionInfo = clientSessionInfo;

            this.HttpRequestEncryptor = new HttpRequestEncryptor<SecureChannelRequestMessage>
                (
                    new SymmetricDataEncryptor<SecureChannelRequestMessage>(clientSessionInfo),
                    new AsymmetricDataEncryptor<SecureChannelRequestMessage>(clientSessionInfo)
                );
        }

        [Inject]
        public IApiHmacKeyResolver ApiHmacKeyResolver { get; set; }

        [Inject]
        public IHttpRequestEncryptor<SecureChannelRequestMessage> HttpRequestEncryptor { get; set; }

        public ClientSessionInfo ClientSessionInfo { get; set; }

        public override Task<HttpRequestMessage> WriteRequestMessageAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            EncryptedServiceProxyInvocationRequest encryptedServiceProxyInvocationRequest = (serviceProxyInvocationRequest as EncryptedServiceProxyInvocationRequest) ?? serviceProxyInvocationRequest.CopyAs<EncryptedServiceProxyInvocationRequest>();
            return WriteRequestMessageAsync(encryptedServiceProxyInvocationRequest);
        }

        public virtual async Task<HttpRequestMessage> WriteRequestMessageAsync(EncryptedServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            HttpRequestMessage httpRequestMessage = await CreateServiceProxyInvocationRequestMessageAsync(serviceProxyInvocationRequest);
            SecureChannelRequestMessage secureChannelRequestMessage = new SecureChannelRequestMessage(serviceProxyInvocationRequest);
            //EncryptedServiceProxyInvocationRequestArgumentWriter argumentWriter = serviceProxyInvocationRequest.ServiceProxyInvocationRequestArgumentWriter as EncryptedServiceProxyInvocationRequestArgumentWriter;

            // Args.ThrowIfNull(argumentWriter, nameof(serviceProxyInvocationRequest.ServiceProxyInvocationRequestArgumentWriter));

            //argumentWriter.ClientSessionInfo = this.ClientSessionInfo;



            //EncryptedServiceProxyInvocationHttpRequestContext descriptor = argumentWriter.WriteEncryptedArgumentContent(httpRequestMessage, secureChannelRequestMessage);
            //ApiSigningKeyResolver
            //ApiSigningKeyResolver.SetSigningKeyToken(descriptor.HttpRequestMessage, descriptor.)
            return httpRequestMessage;
        }
    }
}
