using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvocationRequest<TService> : ServiceProxyInvocationRequest
    {
        public ServiceProxyInvocationRequest(ServiceProxyClient serviceProxyClient, string baseAddress, string className, string methodName, params object[] arguments)
            : base(serviceProxyClient, baseAddress, className, methodName, arguments)
        {
            this.ServiceType = typeof(TService);
        }

        public new ServiceProxyClient<TService> ServiceProxyClient
        {
            get;
            set;
        }

        ServiceProxyInvocationRequestArguments<TService> _serviceProxyArguments;
        public new ServiceProxyInvocationRequestArguments<TService> ServiceProxyArguments
        {
            get
            {
                if (_serviceProxyArguments == null)
                {
                    _serviceProxyArguments = new ServiceProxyInvocationRequestArguments<TService>(this);
                }
                return _serviceProxyArguments;                
            }
        }

        public async Task<TResult> ExecuteAsync<TResult>(ServiceProxyClient<TService> client)
        {
            string response = await ExecuteAsync(client);
            return response.FromJson<TResult>();
        }

        public async Task<string> ExecuteAsync(ServiceProxyClient<TService> client)
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
