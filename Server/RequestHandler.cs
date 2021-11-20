using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public abstract class RequestHandler : IRequestHandler
    {
        public event EventHandler HandleRequestStarted;
        public event EventHandler HandleRequestCompleted;
        public event EventHandler HandleRequestExceptionThrown;

        public IHandleRequestResult HandleRequest(IHttpContext context)
        {
            try
            {
                OnHandleRequestStarted(context);
                IHandleRequestResult result = HandleRequest(context.Request);
                OnHandleRequestCompleted(context);
                return result;
            }
            catch (Exception ex)
            {
                this.OnHandleRequestExceptionThrown(context, ex);
                return new HandleRequestErrorResult { Exception = ex };
            }
        }

        protected void OnHandleRequestStarted(IHttpContext context)
        {
            HandleRequestStarted?.Invoke(this, new RequestHandlerEventArgs { HttpContext = context, RequestHandler = this });
        }

        protected void OnHandleRequestCompleted(IHttpContext context)
        {
            HandleRequestCompleted?.Invoke(this, new RequestHandlerEventArgs { HttpContext = context, RequestHandler = this });
        }

        protected void OnHandleRequestExceptionThrown(IHttpContext context, Exception ex)
        {
            HandleRequestExceptionThrown?.Invoke(this, new RequestHandlerEventArgs { HttpContext = context, RequestHandler = this, Exception = ex });
        }

        protected abstract IHandleRequestResult HandleRequest(IRequest request);
    }
}
