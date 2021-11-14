using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyArguments<TService> : ServiceProxyArguments
    {
        public ServiceProxyArguments(ServiceProxyInvokeRequest request): base(request)
        {
            this.ServiceProxyInvokeRequest = request;
            this.ServiceType = typeof(TService);
            this.ApiArgumentProvider = request.ServiceProxyClient.ApiArgumentProvider;
        }
    }
}
