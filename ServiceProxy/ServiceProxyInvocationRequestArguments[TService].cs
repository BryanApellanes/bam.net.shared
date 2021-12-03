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
            this.ServiceProxyInvocationRequest = request;
            this.ServiceType = typeof(TService);
            this.ApiArgumentEncoder = request.ServiceProxyClient.ApiArgumentEncoder;
        }

        public ServiceProxyInvocationRequestArguments(string methodName, params object[] arguments) : this(new ServiceProxyInvocationRequest<TService>(methodName, arguments))
        {
        }

        public ServiceProxyInvocationRequestArguments(IApiArgumentEncoder apiArgumentEncoder, string methodName, params object[] arguments) : this(new ServiceProxyInvocationRequest<TService>(methodName, arguments))
        {
            this.ApiArgumentEncoder = apiArgumentEncoder;
        }
    }
}
