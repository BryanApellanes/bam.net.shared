using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public class RequestHandlerResolverEventArgs : RequestHandlerEventArgs
    {
        public IRequestHandlerResolver RequestHandlerResolver { get; set; }
    }
}
