using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bam.Net.Server.ServiceProxy
{
    public class CustomEncodingPipelineArgumentReader : ServiceProxyInvocationArgumentReader
    {
        public override ServiceProxyInvocationArgument[] ReadArguments(MethodInfo methodInfo, IRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
