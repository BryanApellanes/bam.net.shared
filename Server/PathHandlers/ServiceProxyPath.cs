using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server.PathHandlers
{
    public class ServiceProxyPath : NamedPath
    {
        public ServiceProxyPath()
        {
            this.PathName = "ServiceProxy";
        }

        public string TypeIdentifier { get; set; }
        public string MethodName { get; set; }

        public new static ServiceProxyPath FromUri(Uri uri)
        {
            ServiceProxyPath serviceProxyPath = FromUri<ServiceProxyPath>(uri);
            serviceProxyPath.TypeIdentifier = serviceProxyPath.Path.ReadUntil('/', out string methodName);
            serviceProxyPath.MethodName = methodName;
            return serviceProxyPath;
        }
    }
}
