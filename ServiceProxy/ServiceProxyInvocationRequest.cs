using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvocationRequest
    {
        public ServiceProxyInvocationRequest(ServiceProxyClient serviceProxyClient, string  baseAddress, string className, string methodName, params object[] arguments)
        {
            this.Cuid = NCuid.Cuid.Generate();
            this.ServiceProxyClient = serviceProxyClient;
            this.ServiceType = serviceProxyClient.ServiceType;
            this.BaseAddress = baseAddress;
            this.ClassName = className;
            
            this.MethodName = methodName;
            this.Arguments = arguments;
        }

        public virtual ServiceProxyClient ServiceProxyClient { get; set; }

        public string Cuid { get; internal set; }

        public Type ServiceType { get; set; }

        public string BaseAddress { get; set; }

        string _className;
        public virtual string ClassName
        {
            get
            {
                if (string.IsNullOrEmpty(_className))
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

        HashSet<string> _methods;
        object _methodsLock = new object();
        public HashSet<string> Methods
        {
            get
            {
                return _methodsLock.DoubleCheckLock(ref _methods, () => new HashSet<string>(ServiceProxySystem.GetProxiedMethods(ServiceType).Select(m => m.Name).ToArray()));
            }
        }

        public virtual string MethodName { get; set; }

        public object[] Arguments { get; set; }
        
        public virtual ServiceProxyVerbs Verb 
        {
            get
            {
                return ServiceProxyClient.GetServiceProxyInvocationUrl(this, ServiceProxyInvocationRequestArguments)?.Length >= 2048 ? ServiceProxyVerbs.Post: ServiceProxyVerbs.Get;
            }
        }

        ServiceProxyInvocationRequestArguments _serviceProxyArguments;
        public virtual ServiceProxyInvocationRequestArguments ServiceProxyInvocationRequestArguments
        {
            get
            {
                if (_serviceProxyArguments == null)
                {
                    _serviceProxyArguments = new ServiceProxyInvocationRequestArguments(this);
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
