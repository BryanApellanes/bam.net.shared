using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvocationRequestArgumentWriter<TService> : ServiceProxyInvocationRequestArgumentWriter
    {
        public ServiceProxyInvocationRequestArgumentWriter(ServiceProxyInvocationRequest request): base(request)
        {
            this.ServiceProxyInvocationRequest = request;
            this.ServiceType = typeof(TService);
            this.ApiArgumentEncoder = request.ServiceProxyClient.ApiArgumentEncoder;
        }

        public ServiceProxyInvocationRequestArgumentWriter(string methodName, params object[] arguments) : this(new ServiceProxyInvocationRequest<TService>(methodName, arguments))
        {
        }

        public ServiceProxyInvocationRequestArgumentWriter(IApiArgumentEncoder apiArgumentEncoder, string methodName, params object[] arguments) : this(new ServiceProxyInvocationRequest<TService>(methodName, arguments))
        {
            this.ApiArgumentEncoder = apiArgumentEncoder;
        }
    }
}
