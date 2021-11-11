/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Bam.Net.ServiceProxy
{
    /// <summary>
    /// Encapsulates method and parameters for 
    /// a ServiceProxy call.
    /// </summary>
    public class ServiceProxyArguments
    {
        public ServiceProxyArguments(ServiceProxyInvokeRequest invokeRequest)
        {
            this.ServiceType = invokeRequest.ServiceType;
            this.MethodName = invokeRequest.MethodName;
            this.Arguments = invokeRequest.Arguments;
            this.MethodInfo = GetMethodInfo();
        }

        public ServiceProxyArguments(Type serviceType, MethodInfo method, object[] arguments)
        {
            this.ServiceType = serviceType;
            this.MethodInfo = method;
            this.Arguments = arguments;
            this.ApiArgumentProvider = DefaultApiArgumentProvider.Current;
        }

        public virtual IApiArgumentProvider ApiArgumentProvider
        {
            get;
            set;
        }

        public Type ServiceType { get; set; }

        string _methodName;
        public string MethodName
        {
            get
            {
                if (string.IsNullOrEmpty(_methodName) && MethodInfo != null)
                {
                    _methodName = MethodInfo.Name;
                }
                return _methodName;
            }
            set
            {
                _methodName = value;
            }
        }

        public MethodInfo MethodInfo
        {
            get;
            set;
        }

        public object[] Arguments
        {
            get;
            set;
        }

        public string NumberedQueryStringParameters
        {
            get
            {
                return ApiArgumentProvider.ArgumentsToNumberedQueryString(Arguments);
            }
        }

        public virtual ServiceProxyVerbs Verb
        {
            get
            {
                return NumberedQueryStringParameters?.Length > 2048 ? ServiceProxyVerbs.Post : ServiceProxyVerbs.Get;
            }
        }

        public virtual MethodInfo GetMethodInfo()
        {
            return ServiceType.GetMethod(MethodName, Arguments.Select(argument => argument.GetType()).ToArray());
        }

        public string GetJsonArgsMember()
        {
            return this.ApiArgumentProvider.ArgumentsToJsonArgsMember(Arguments);
        }

        public string[] GetJsonArgumentsArray()
        {
            return this.ApiArgumentProvider.ArgumentsToJsonArgumentsArray(Arguments);
        }
    }
}
