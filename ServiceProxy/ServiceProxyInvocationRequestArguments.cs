/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net.Http;

namespace Bam.Net.ServiceProxy
{
    /// <summary>
    /// Encapsulates method and parameters for 
    /// a ServiceProxy call.
    /// </summary>
    public class ServiceProxyInvocationRequestArguments
    {
        public const string JsonMediaType = "application/json; charset=utf-8";

        public ServiceProxyInvocationRequestArguments(ServiceProxyInvocationRequest invokeRequest)
        {
            this.ServiceProxyInvokeRequest = invokeRequest;
            this.ServiceType = invokeRequest.ServiceType;
            this.MethodName = invokeRequest.MethodName;
            this.Arguments = invokeRequest.Arguments;
            this.MethodInfo = GetMethodInfo();
            this.ApiArgumentProvider = DefaultApiArgumentProvider.Current;
        }

        public virtual IApiArgumentProvider ApiArgumentProvider
        {
            get;
            set;
        }

        public ServiceProxyInvocationRequest ServiceProxyInvokeRequest
        {
            get;
            protected set;
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

        public Dictionary<string, object> NamedArguments
        {
            get
            {
                return ApiArgumentProvider.GetNamedArguments(MethodInfo, Arguments);
            }
        }

        /// <summary>
        /// NamedArguments as URL encoded key value pairs.
        /// </summary>
        public string QueryStringArguments
        {
            get
            {
                return ApiArgumentProvider.ArgumentsToQueryString(NamedArguments);
            }
        }

        public virtual void SetContent(HttpRequestMessage requestMessage)
        {
            string jsonArgsMember = GetJsonArgsMember();
            requestMessage.Content = new StringContent(jsonArgsMember, Encoding.UTF8, JsonMediaType);
        }

        public virtual MethodInfo GetMethodInfo()
        {
            return ServiceType.GetMethod(MethodName, Arguments.Select(argument => argument.GetType()).ToArray());
        }

        /// <summary>
        /// Gets an anonymous object that represents a json member with the name of `jsonArgs` whose 
        /// value is a json serialized array of json strings.
        /// </summary>
        /// <returns></returns>
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