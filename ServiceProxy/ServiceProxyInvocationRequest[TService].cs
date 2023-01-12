using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvocationRequest<TService> : ServiceProxyInvocationRequest
    {
        public ServiceProxyInvocationRequest(ServiceProxyClient serviceProxyClient, string methodName, params object[] arguments)
            : base(serviceProxyClient, typeof(TService).Name, methodName, arguments)
        {
            this.ServiceType = typeof(TService);
        }

        public ServiceProxyInvocationRequest(string methodName, params object[] arguments)
            : this(new ServiceProxyClient<TService>(), methodName, arguments)
        {
        }

        public new ServiceProxyClient<TService> ServiceProxyClient
        {
            get;
            set;
        }

        ServiceProxyInvocationRequestArgumentWriter<TService> _serviceProxyArguments;
        public new ServiceProxyInvocationRequestArgumentWriter<TService> ServiceProxyInvocationRequestArgumentWriter
        {
            get
            {
                if (_serviceProxyArguments == null)
                {
                    _serviceProxyArguments = new ServiceProxyInvocationRequestArgumentWriter<TService>(this);
                }
                return _serviceProxyArguments;                
            }
        }

        public async Task<HttpClientResponse> ExecuteAsync(ServiceProxyClient<TService> client)
        {
            if (!Methods.Contains(MethodName))
            {
                throw Args.Exception<InvalidOperationException>("{0} is not proxied from type {1}", MethodName, typeof(TService).Name);
            }
            this.ServiceProxyClient = client;
            if (Verb == ServiceProxyVerbs.Post)
            {
                return await client.ReceivePostResponseAsync(this);
            }
            else
            {
                return await client.ReceiveGetResponseAsync(this);
            }
        }
    }
}
