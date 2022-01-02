using Bam.Net.ServiceProxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bam.Net.Server.ServiceProxy
{
    public abstract class ServiceProxyInvocationArgumentReader
    {
        public const string JsonArgsMemberName = "jsonArgs";

        [JsonProperty(JsonArgsMemberName)]
        public string JsonArgsMember { get; set; }

        public virtual ServiceProxyInvocationArgument DecodeArgument(ParameterInfo parameterInfo, string jsonValue)
        {
            return new ServiceProxyInvocationArgument(this, parameterInfo, jsonValue);
        }

        public virtual object DecodeValue(Type type, string encodedValue)
        {
            return encodedValue.FromJson(type);
        }

        /// <summary>
        /// Set the arguments on the specified service proxy invocation by reading them from the specified request.
        /// </summary>
        /// <param name="serviceProxyInvocation"></param>
        /// <param name="request"></param>
        public void SetArguments(ServiceProxyInvocation serviceProxyInvocation, IRequest request)
        {
            serviceProxyInvocation.Arguments = ReadArguments(serviceProxyInvocation, request);
        }

        public ServiceProxyInvocationArgument[] ReadArguments(ServiceProxyInvocation serviceProxyInvocation, IRequest request)
        {
            return ReadArguments(serviceProxyInvocation.MethodInfo, request);
        }

        public abstract ServiceProxyInvocationArgument[] ReadArguments(MethodInfo methodInfo, IRequest request);

        public virtual ServiceProxyInvocationArgument[] ReadJsonArgumentsMember(MethodInfo methodInfo, string body)
        {
            InputStreamServiceProxyInvocationArgumentReader arguments = body.FromJson<InputStreamServiceProxyInvocationArgumentReader>();

            List<ParameterInfo> parameters = methodInfo.GetParameters().ToList();
            parameters.Sort((x, y) => x.Position.CompareTo(y.Position));

            string[] arrayOfJsonArguments = arguments.JsonArgsMember.FromJson<string[]>();

            List<ServiceProxyInvocationArgument> results = new List<ServiceProxyInvocationArgument>();
            for (int i = 0; i < parameters.Count; i++)
            {
                ParameterInfo parameter = parameters[i];
                string jsonArgument = arrayOfJsonArguments[i];
                results.Add(new ServiceProxyInvocationArgument(this, parameter, jsonArgument));
            }
            return results.ToArray();
        }
    }
}
