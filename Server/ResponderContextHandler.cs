using Bam.Net.Server.PathHandlers;
using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server
{
    public abstract class ResponderContextHandler<TResponder>
        : IHttpContextHandler, IContextCloneable, IRequiresHttpContext
        where TResponder: Responder
    {
        public event EventHandler HandleContextStarted;
        public event EventHandler HandleContextCompleted;
        public event EventHandler HandleContextExceptionThrown;

        internal ResponderContextHandler() 
        { }

        public NamedPath NamedPath { get; set; }

        public TResponder Responder { get; set; }

        public IHttpContext HttpContext
        {
            get;
            set;
        }

        public Task<IHttpResponse> HandleContextAsync(IHttpContext context)
        {
            try
            {
                OnHandleContextStarted(context);
                IHttpResponse result = HandleContext(context);
                OnHandleContextCompleted(context);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                this.OnHandleContextExceptionThrown(context, ex);
                return Task.FromResult((IHttpResponse)new HttpErrorResponse(ex));
            }
        }

        protected void OnHandleContextStarted(IHttpContext context)
        {
            HandleContextStarted?.Invoke(this, new RequestHandlerEventArgs<TResponder> { HttpContext = context, RequestHandler = this });
        }

        protected void OnHandleContextCompleted(IHttpContext context)
        {
            HandleContextCompleted?.Invoke(this, new RequestHandlerEventArgs<TResponder> { HttpContext = context, RequestHandler = this });
        }

        protected void OnHandleContextExceptionThrown(IHttpContext context, Exception ex)
        {
            HandleContextExceptionThrown?.Invoke(this, new RequestHandlerEventArgs<TResponder> { HttpContext = context, RequestHandler = this, Exception = ex });
        }

        protected abstract IHttpResponse HandleContext(IHttpContext context);

        public abstract object Clone();

        public object Clone(IHttpContext context)
        {
            object clone = Clone();
            if (clone is IRequiresHttpContext requiresHttpContext)
            {
                requiresHttpContext.HttpContext = context;
                return requiresHttpContext;
            }

            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }
    }
}
