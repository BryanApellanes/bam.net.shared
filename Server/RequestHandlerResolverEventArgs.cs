using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public class RequestHandlerResolverEventArgs<TResponder> : RequestHandlerEventArgs<TResponder>
        where TResponder : Responder
    {
        public IContextHandlerResolver<TResponder> RequestHandlerResolver { get; set; }
    }
}
