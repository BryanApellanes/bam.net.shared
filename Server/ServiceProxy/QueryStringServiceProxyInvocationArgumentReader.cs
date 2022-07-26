using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server.ServiceProxy
{
    public class QueryStringServiceProxyInvocationArgumentReader : ServiceProxyInvocationArgumentReader
    {
        public override async Task<ServiceProxyInvocationArgument[]> ReadArgumentsAsync(MethodInfo methodInfo, IHttpContext context)
        {
            List<ServiceProxyInvocationArgument> arguments = new List<ServiceProxyInvocationArgument>();
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            foreach(ParameterInfo parameterInfo in parameterInfos)
            {
                string jsonArgument = context.Request.QueryString[parameterInfo.Name];
                arguments.Add(DecodeArgument(parameterInfo, jsonArgument));
            }
            return await Task.FromResult(arguments.ToArray());
        }
    }
}
