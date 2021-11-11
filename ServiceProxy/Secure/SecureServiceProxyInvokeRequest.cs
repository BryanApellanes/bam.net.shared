using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Secure
{
    public class SecureServiceProxyInvokeRequest<TService> : ServiceProxyInvokeRequest
    {
        public SecureServiceProxyInvokeRequest() : base(typeof(TService))
        {
        }
    }
}
