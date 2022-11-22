using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Net.Server.PathHandlers;
using Bam.Net.ServiceProxy;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server.ServiceProxy
{
    public interface IServiceProxyInvocationReader
    {
        ServiceProxyInvocation ReadServiceProxyInvocation(ServiceProxyPath serviceProxyPath, WebServiceProxyDescriptors webServiceProxyDescriptors, IHttpContext context);
    }
}
