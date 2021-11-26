using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bam.Net.Server.ServiceProxy
{
    public class QueryStringServiceProxyInvocationArgumentReader : ServiceProxyInvocationArgumentReader
    {
        public override ServiceProxyInvocationArgument[] ReadArguments(MethodInfo methodInfo, IRequest request)
        {
            List<ServiceProxyInvocationArgument> arguments = new List<ServiceProxyInvocationArgument>();
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            foreach(ParameterInfo parameterInfo in parameterInfos)
            {
                string jsonArgument = request.QueryString[parameterInfo.Name];
                arguments.Add(DecodeArgument(parameterInfo, jsonArgument));
            }
            return arguments.ToArray();
        }
    }
}
