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
        public ServiceProxyInvocationRequest() { }
        public ServiceProxyInvocationRequest(ServiceProxyClient serviceProxyClient, string className, string methodName, params object[] arguments)
        {
            this.Cuid = Bam.Net.Cuid.Generate();
            this.ServiceProxyClient = serviceProxyClient;
            this.ServiceType = serviceProxyClient.ServiceType;
            this.BaseAddress = serviceProxyClient.BaseAddress;
            this.ClassName = className;
            
            this.MethodName = methodName;
            this.Arguments = arguments;

            this.MethodUrlFormat = "{BaseAddress}ServiceProxy/Invoke/{ClassName}/{MethodName}?{QueryStringArguments}";
        }

        public virtual ServiceProxyClient ServiceProxyClient { get; set; }
        public string MethodUrlFormat { get; set; }
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

        /// <summary>
        /// Gets or sets the arguments intended for the method invocation.
        /// </summary>
        public object[] Arguments { get; set; }

        ServiceProxyVerbs _verb;
        public virtual ServiceProxyVerbs Verb 
        {
            get
            {
                if (_verb == ServiceProxyVerbs.Invalid)
                {
                    // 2048 is a somewhat arbitrary value that is the longest Url Internet Explorer can accept.
                    // ServiceProxyClient and derivatives use HttpClient under the hood which 
                    // shouldn't have this limitation. Specifying this limit helps provide broader support
                    // in the future to display results in a browser when rendering responses natively as html.
                    _verb = GetInvocationUrl()?.Length >= 2048 ? ServiceProxyVerbs.Post : ServiceProxyVerbs.Get;
                }
                return _verb;
            }
            set
            {
                _verb = value;
            }
        }

        ServiceProxyInvocationRequestArgumentWriter _serviceProxyArgumentWriter;
        public virtual ServiceProxyInvocationRequestArgumentWriter ServiceProxyInvocationRequestArgumentWriter
        {
            get
            {
                if (_serviceProxyArgumentWriter == null)
                {
                    _serviceProxyArgumentWriter = new ServiceProxyInvocationRequestArgumentWriter(this);
                }
                return _serviceProxyArgumentWriter;
            }
        }

        public virtual string GetInvocationUrl(bool includeQueryString = true, ServiceProxyClient serviceProxyClient = null)
        {
            return MethodUrlFormat.NamedFormat(new
            {
                BaseAddress = serviceProxyClient?.BaseAddress ?? BaseAddress,
                ClassName,
                MethodName,
                QueryStringArguments = includeQueryString ? ServiceProxyInvocationRequestArgumentWriter?.QueryStringArguments: "",
            });
        }

/*        public virtual ServiceProxyClient GetClient()
        {
            this.ServiceProxyClient = this.ServiceProxyClient ?? this.CopyAs<ServiceProxyClient>(this.ServiceType, this.BaseAddress);
            return this.ServiceProxyClient;
        }*/

        /*public async Task<TResult> ExecuteAsync<TService, TResult>(params object[] arguments)
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
        }*/
    }
}
