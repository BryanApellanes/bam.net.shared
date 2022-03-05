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
    public class ServiceProxyInvocationRequestArgumentWriter
    {
        public ServiceProxyInvocationRequestArgumentWriter(ServiceProxyInvocationRequest invocationRequest, IApiArgumentEncoder apiArgumentEncoder = null)
        {
            this.ServiceProxyInvocationRequest = invocationRequest;
            this.ServiceType = invocationRequest.ServiceType;
            this.MethodName = invocationRequest.MethodName;
            this.Arguments = invocationRequest.Arguments;
            this.MethodInfo = GetMethodInfo();
            this.ApiArgumentEncoder = apiArgumentEncoder ?? DefaultApiArgumentEncoder.Current;
        }

        public virtual IApiArgumentEncoder ApiArgumentEncoder
        {
            get;
            set;
        }

        public ServiceProxyInvocationRequest ServiceProxyInvocationRequest
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
                return ApiArgumentEncoder.GetNamedArguments(MethodInfo, Arguments);
            }
        }

        /// <summary>
        /// NamedArguments as query string key value pairs.
        /// </summary>
        public string QueryStringArguments
        {
            get
            {
                return ApiArgumentEncoder.ArgumentsToQueryString(NamedArguments);
            }
        }

        public virtual void WriteArguments(HttpRequestMessage requestMessage)
        {
            if(requestMessage.Method == HttpMethod.Get)
            {
                WriteArgumentQueryString(requestMessage);
            }
            else
            {
                WriteArgumentContent(requestMessage);
            }

        }

        public virtual void WriteArgumentContent(HttpRequestMessage requestMessage)
        {
            string jsonArgsMember = GetJsonArgsMember();
            requestMessage.Content = new StringContent(jsonArgsMember, Encoding.UTF8, MediaTypes.Json);
        }

        public virtual void WriteArgumentQueryString(HttpRequestMessage requestMessage)
        {
            Uri currentUri = requestMessage.RequestUri;
            Uri newUri = new Uri($"{currentUri.Scheme}://{currentUri.Authority}{currentUri.AbsolutePath}?{QueryStringArguments}");
            requestMessage.RequestUri = newUri;
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
            return this.ApiArgumentEncoder.ArgumentsToJsonArgsMember(Arguments);
        }

        public string[] GetJsonArgumentsArray()
        {
            return this.ApiArgumentEncoder.ArgumentsToJsonArgumentsArray(Arguments);
        }
    }
}
