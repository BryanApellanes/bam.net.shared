using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvocationRequestArguments<TService> : ServiceProxyInvocationRequestArguments
    {
        public ServiceProxyInvocationRequestArguments(ServiceProxyInvocationRequest request): base(request)
        {
            this.ServiceProxyInvokeRequest = request;
            this.ServiceType = typeof(TService);
            this.ApiArgumentProvider = request.ServiceProxyClient.ApiArgumentProvider;
        }
    }
}
