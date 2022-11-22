using Bam.Net.ServiceProxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public virtual ServiceProxyInvocation CreateServiceProxyInvocation(WebServiceProxyDescriptors webServiceProxyDescriptors, string className, string methodName, IHttpContext context)
        {
            return new ServiceProxyInvocation(webServiceProxyDescriptors, className, methodName, context);
        }

        public abstract Task<ServiceProxyInvocationArgument[]> ReadArgumentsAsync(MethodInfo methodInfo, IHttpContext httpContext);

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
