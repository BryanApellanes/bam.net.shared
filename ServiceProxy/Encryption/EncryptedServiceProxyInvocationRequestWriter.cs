using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class EncryptedServiceProxyInvocationRequestWriter : ServiceProxyInvocationRequestWriter
    {
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
            argumentWriter.ClientSessionInfo = this.ClientSessionInfo;
            argumentWriter.WriteArguments(httpRequestMessage);
            
            return httpRequestMessage;
        }
    }
}
