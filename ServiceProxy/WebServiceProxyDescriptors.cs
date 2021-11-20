using Bam.Net.Services;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    /// <summary>
    /// Descriptor for web service proxies and client side aliases.
    /// </summary>
    public class WebServiceProxyDescriptors
    {
        /// <summary>
        /// Gets or sets the WebServiceRegistry.
        /// </summary>
        public WebServiceRegistry WebServiceRegistry { get; set; }

        /// <summary>
        /// Gets or sets the aliases for types contained in the WebServiceRegistry.
        /// </summary>
        public HashSet<ProxyAlias> ProxyAliases { get; set; }
    }
}
