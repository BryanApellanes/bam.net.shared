using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public interface IRequestHandlerResolver
    {
        event EventHandler<RequestHandlerResolverEventArgs> ResolveHandlerStarted;
        event EventHandler<RequestHandlerResolverEventArgs> ResolveHandlerComplete;
        event EventHandler<RequestHandlerResolverEventArgs> ResolveHandlerExceptionThrown;

        RequestHandler ResolveHandler(IHttpContext context);
    }
}
