using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyArguments<TService> : ServiceProxyArguments
    {
        public ServiceProxyArguments(ServiceProxyInvokeRequest<TService> request)
            : this(request.MethodName, request.Arguments)
        {
            ServiceType = typeof(TService);
            this.ApiArgumentProvider = request.ServiceProxyClient.ApiArgumentProvider;
        }

        public ServiceProxyArguments(string methodName, object[] arguments) 
            : base
            (
                  typeof(TService),
                  typeof(TService).GetMethod(methodName, arguments.Select(arg => arg.GetType()).ToArray()), 
                  arguments
            )
        {
            ServiceType = typeof(TService);
            this.ApiArgumentProvider = DefaultApiArgumentProvider<TService>.Current;
        }

        public ServiceProxyArguments(MethodInfo method, object[] arguments) : base(typeof(TService), method, arguments)
        {
            this.ServiceType = typeof(TService);
            this.ApiArgumentProvider = DefaultApiArgumentProvider<TService>.Current;
        }

        protected new IApiArgumentProvider<TService> ApiArgumentProvider // TODO: inject this
        {
            get;
            set;
        }

        public Dictionary<string, object> NamedArguments
        {
            get
            {
                return ApiArgumentProvider.GetNamedArguments(MethodInfo, Arguments);
            }
        }

        public string QueryStringArguments
        {
            get
            {
                return ApiArgumentProvider.ArgumentsToQueryString(NamedArguments);
            }
        }

        public override ServiceProxyVerbs Verb
        {
            get
            {
                return QueryStringArguments?.Length > 2048 ? ServiceProxyVerbs.Post : ServiceProxyVerbs.Get;
            }
        }
    }
}
