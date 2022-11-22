using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server.ServiceProxy
{
    public class InputStreamServiceProxyInvocationArgumentReader : ServiceProxyInvocationArgumentReader
    {
        public override async Task<ServiceProxyInvocationArgument[]> ReadArgumentsAsync(MethodInfo methodInfo, IHttpContext context)
        {
            string body = context.Request.InputStream.ReadToEnd();
            return await Task.FromResult(ReadJsonArgumentsMember(methodInfo, body));
        }
    }
}
