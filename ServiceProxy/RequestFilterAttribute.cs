using Bam.Net.Server.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class RequestFilterAttribute: Attribute
    {
        public abstract bool RequestIsAllowed(ServiceProxyInvocation request, out string failureMessage);
    }
}
