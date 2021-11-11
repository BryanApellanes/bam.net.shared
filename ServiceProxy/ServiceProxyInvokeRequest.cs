using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvokeRequest
    {
        public ServiceProxyInvokeRequest()
        {
            Cuid = NCuid.Cuid.Generate();
        }

        public ServiceProxyInvokeRequest(Type serviceType): this()
        {
            this.ServiceType = serviceType;
        }

        public virtual ServiceProxyClient ServiceProxyClient { get; set; }

        public string Cuid { get; internal set; }

        public Type ServiceType { get; set; }

        public string BaseAddress { get; set; }

        public virtual string ClassName
        {
            get; set; 
        }

        public virtual string MethodName { get; set; }

        public object[] Arguments { get; set; }

        ServiceProxyArguments _serviceProxyArguments;
        public virtual ServiceProxyArguments ServiceProxyArguments
        {
            get
            {
                if (_serviceProxyArguments == null)
                {
                    _serviceProxyArguments = new ServiceProxyArguments(this);
                }
                return _serviceProxyArguments;
            }
        }

        public virtual ServiceProxyClient GetClient()
        {
            this.ServiceProxyClient = this.ServiceProxyClient ?? this.CopyAs<ServiceProxyClient>(this.ServiceType, this.BaseAddress);
            return this.ServiceProxyClient;
        }

        public async Task<TResult> ExecuteAsync<TService, TResult>(params object[] arguments)
        {
            this.ServiceType = typeof(TService);
            string response = await this.ExecuteAsync(this.GetClient());
            return response.FromJson<TResult>();
        }

        public async Task<string> ExecuteAsync(ServiceProxyClient client)
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
