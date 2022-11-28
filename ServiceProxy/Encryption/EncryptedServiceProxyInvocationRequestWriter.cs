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
        public EncryptedServiceProxyInvocationRequestWriter(IClientKeySource clientKeySource, IApiHmacKeyResolver apiHmacKeyResolver)
        {
            this.ApiHmacKeyResolver = apiHmacKeyResolver;
            this.ClientKeySource = clientKeySource;

            this.HttpRequestEncryptor = new HttpRequestEncryptor<SecureChannelRequestMessage>
                (
                    new SymmetricContentEncryptor<SecureChannelRequestMessage>(clientKeySource),
                    new AsymmetricDataEncryptor<SecureChannelRequestMessage>(clientKeySource)
                );
        }

        [Inject]
        public IApiHmacKeyResolver ApiHmacKeyResolver { get; set; }

        [Inject]
        public IHttpRequestEncryptor<SecureChannelRequestMessage> HttpRequestEncryptor { get; set; }

        public IClientKeySource ClientKeySource { get; set; }

        public override async Task<HttpRequestMessage> WriteRequestMessageAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            EncryptedServiceProxyInvocationRequest encryptedServiceProxyInvocationRequest = (serviceProxyInvocationRequest as EncryptedServiceProxyInvocationRequest) ?? serviceProxyInvocationRequest.CopyAs<EncryptedServiceProxyInvocationRequest>();
            return await WriteRequestMessageAsync(encryptedServiceProxyInvocationRequest);
        }

        public virtual async Task<HttpRequestMessage> WriteRequestMessageAsync(EncryptedServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            HttpRequestMessage httpRequestMessage = await CreateServiceProxyInvocationRequestMessageAsync(serviceProxyInvocationRequest);

            HttpRequest<SecureChannelRequestMessage> request = HttpRequest.FromHttpRequestMessage<SecureChannelRequestMessage>(httpRequestMessage);
            request.TypedContent = new SecureChannelRequestMessage(serviceProxyInvocationRequest);
            IHttpRequest encryptedRequest = HttpRequestEncryptor.EncryptRequest(request);
            return encryptedRequest.ToHttpRequestMessage();
        }
    }
}
