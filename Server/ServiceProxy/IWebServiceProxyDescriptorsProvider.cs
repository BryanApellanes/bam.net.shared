using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace  Bam.Net.Server.ServiceProxy
{
    public interface IWebServiceProxyDescriptorsProvider
    {
        WebServiceProxyDescriptors GetWebServiceProxyDescriptors(string applicationName);
    }
}
