using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bam.Net.Server.ServiceProxy
{
    public class InputStreamServiceProxyInvocationArgumentReader : ServiceProxyInvocationArgumentReader
    {
        public override ServiceProxyInvocationArgument[] ReadArguments(MethodInfo methodInfo, IRequest request)
        {
            string body = request.InputStream.ReadToEnd();
            return ReadJsonArgumentsMember(methodInfo, body);
        }
    }
}
