using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class EncryptedServiceProxyInvocationRequestWriter : ServiceProxyInvocationRequestWriter
    {
        public override Task<HttpRequestMessage> WriteRequestMessageAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            throw new NotImplementedException();
        }
    }
}
