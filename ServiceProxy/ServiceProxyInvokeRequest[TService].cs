using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvokeRequest<TService> : ServiceProxyInvokeRequest
    {
        public ServiceProxyInvokeRequest()
        {
            this.ServiceType = typeof(TService);
        }

        public ServiceProxyInvokeRequest(HttpClient httpClient): this()
        {
            this.ServiceProxyClient = new ServiceProxyClient<TService>(httpClient);
        }

        public ServiceProxyInvokeRequest(ServiceProxyClient<TService> client) : this()
        {
            this.ServiceProxyClient = client;
        }

        public new ServiceProxyClient<TService> ServiceProxyClient
        {
            get;
            set;
        }

        string _className;
        public override string ClassName 
        {
            get
            {
                if(string.IsNullOrEmpty(_className))
                {
                    _className = ServiceType?.Name;
                }
                return _className;
            }
            set
            {
                _className = value;
            }
        }

        ServiceProxyArguments<TService> _serviceProxyArguments;
        public new ServiceProxyArguments<TService> ServiceProxyArguments
        {
            get
            {
                if (_serviceProxyArguments == null)
                {
                    _serviceProxyArguments = new ServiceProxyArguments<TService>(this);
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
            this.ServiceProxyClient = client;
            if (ServiceProxyArguments.Verb == ServiceProxyVerbs.Post)
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
