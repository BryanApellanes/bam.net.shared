using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public abstract class RequestHandlerResolver : IRequestHandlerResolver
    {
        public event EventHandler<RequestHandlerResolverEventArgs> ResolveHandlerStarted;
        public event EventHandler<RequestHandlerResolverEventArgs> ResolveHandlerComplete;
        public event EventHandler<RequestHandlerResolverEventArgs> ResolveHandlerExceptionThrown;

        public RequestHandler ResolveHandler(IHttpContext httpContext)
        {
            try
            {
                Args.ThrowIfNull(httpContext, "context");
                OnResolveHandlerStarted(httpContext);
                RequestHandler requestHandler = ResolveHandler(httpContext.Request);
                OnResolveHandlerComplete(httpContext, requestHandler);
                return requestHandler;
            }
            catch (Exception ex)
            {
                OnResolveHandlerExceptionThrown(httpContext, ex);
                return null;
            }
        }

        protected void OnResolveHandlerStarted(IHttpContext httpContext)
        {
            ResolveHandlerStarted?.Invoke(this, new RequestHandlerResolverEventArgs { HttpContext = httpContext, RequestHandlerResolver = this });
        }

        protected void OnResolveHandlerComplete(IHttpContext httpContext, RequestHandler requestHandler)
        {
            ResolveHandlerComplete?.Invoke(this, new RequestHandlerResolverEventArgs { HttpContext = httpContext, RequestHandlerResolver = this, RequestHandler = requestHandler });
        }

        protected void OnResolveHandlerExceptionThrown(IHttpContext httpContext, Exception ex)
        {
            ResolveHandlerExceptionThrown?.Invoke(this, new RequestHandlerResolverEventArgs { HttpContext = httpContext, RequestHandlerResolver = this, Exception = ex });
        }

        protected abstract RequestHandler ResolveHandler(IRequest request);
    }
}
