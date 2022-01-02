using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server.ServiceProxy
{
    public class ProxyCodeContextHandler : ResponderContextHandler<ServiceProxyResponder>
    {
        public override object Clone()
        {
            ProxyCodeContextHandler proxyCodeRequestHandler = new ProxyCodeContextHandler();
            proxyCodeRequestHandler.CopyProperties(this);
            proxyCodeRequestHandler.CopyEventHandlers(this);
            return proxyCodeRequestHandler;
        }

        protected override IHttpResponse HandleContext(IHttpContext context)
        {
            // See ServiceProxyResponder.SendProxyCode
            throw new NotImplementedException();
        }
    }
}
