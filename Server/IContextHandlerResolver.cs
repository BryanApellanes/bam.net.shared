using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public interface IContextHandlerResolver<TResponder> where TResponder : Responder
    {
        event EventHandler<RequestHandlerResolverEventArgs<TResponder>> ResolveHandlerStarted;
        event EventHandler<RequestHandlerResolverEventArgs<TResponder>> ResolveHandlerComplete;
        event EventHandler<RequestHandlerResolverEventArgs<TResponder>> ResolveHandlerExceptionThrown;

        ResponderContextHandler<TResponder> ResolveHandler(IHttpContext context);
    }
}
