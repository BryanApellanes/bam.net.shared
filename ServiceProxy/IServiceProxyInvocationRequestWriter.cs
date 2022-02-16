using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public interface IServiceProxyInvocationRequestWriter
    {
        Task<HttpRequestMessage> WriteRequestMessageAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest);
    }
}
