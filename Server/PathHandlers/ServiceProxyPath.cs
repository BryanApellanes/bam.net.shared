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

        /// <summary>
        /// Gets or sets the type identifier, typically the class name.
        /// </summary>
        public string TypeIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the method name.
        /// </summary>
        public string MethodName { get; set; }

        public new static ServiceProxyPath FromUri(Uri uri)
        {
            ServiceProxyPath serviceProxyPath = FromUri<ServiceProxyPath>(uri);
            if (serviceProxyPath.Path.StartsWith("Invoke", StringComparison.InvariantCultureIgnoreCase))
            {
                serviceProxyPath.Path = serviceProxyPath.Path.TruncateFront("Invoke/".Length);
            }
            serviceProxyPath.TypeIdentifier = serviceProxyPath.Path.ReadUntil('/', out string methodName);
            serviceProxyPath.MethodName = methodName;
            return serviceProxyPath;
        }
    }
}
