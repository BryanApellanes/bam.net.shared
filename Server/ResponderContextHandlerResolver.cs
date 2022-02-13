using Bam.Net.Server.PathHandlers;
using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    internal class ResponderContextHandlerResolver<TResponder> : IContextHandlerResolver<TResponder> where TResponder: Responder
    {
        public event EventHandler<RequestHandlerResolverEventArgs<TResponder>> ResolveHandlerStarted;
        public event EventHandler<RequestHandlerResolverEventArgs<TResponder>> ResolveHandlerComplete;
        public event EventHandler<RequestHandlerResolverEventArgs<TResponder>> ResolveHandlerExceptionThrown;

        Dictionary<string, Func<IHttpContext, NamedPath, ResponderContextHandler<TResponder>>> _pathRequestHandlers;
        Func<IHttpContext, NamedPath, ResponderContextHandler<TResponder>> _defaultRequestHandler;
        public ResponderContextHandlerResolver(TResponder responder)
        {
            this._pathRequestHandlers = new Dictionary<string, Func<IHttpContext, NamedPath, ResponderContextHandler<TResponder>>>();
            this.Responder = responder;
        }

        public TResponder Responder { get; set; }

        public void AddPathNameHandler<TRequestHandler>(string pathName,bool isDefault = false) where TRequestHandler : ResponderContextHandler<TResponder>, new()
        {
            Func<IHttpContext, NamedPath, ResponderContextHandler<TResponder>> requestHandler = (httpContext, namedPathRoute) => new TRequestHandler { NamedPath = namedPathRoute, Responder = Responder };
            _pathRequestHandlers.Add(pathName.ToLowerInvariant(), requestHandler);
            if (isDefault)
            {
                _defaultRequestHandler = requestHandler;
            }
        }

        protected Func<IHttpContext, ResponderContextHandler<TResponder>> ResolveHandler(IRequest request)
        {
            Args.ThrowIfNull(request, nameof(request));

            ServiceProxyPath serviceProxyPath = ServiceProxyPath.FromUri(request.Url);
            if (Responder.ResponderName.Equals(serviceProxyPath.PathName, StringComparison.InvariantCultureIgnoreCase))
            {
                string typeIdentifier = serviceProxyPath.TypeIdentifier;
                if (_pathRequestHandlers.ContainsKey(typeIdentifier))
                {
                    return (httpContext) => _pathRequestHandlers[typeIdentifier](httpContext, serviceProxyPath);
                }
            }

            return (httpContext) => _defaultRequestHandler(httpContext, serviceProxyPath);            
        }

        public ResponderContextHandler<TResponder> ResolveHandler(IHttpContext httpContext)
        {
            try
            {
                Args.ThrowIfNull(httpContext, nameof(httpContext));
                OnResolveHandlerStarted(httpContext);
                Func<IHttpContext, ResponderContextHandler<TResponder>> requestHandlerFunc = ResolveHandler(httpContext.Request);
                ResponderContextHandler<TResponder> requestHandler = requestHandlerFunc(httpContext);
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
            ResolveHandlerStarted?.Invoke(this, new RequestHandlerResolverEventArgs<TResponder> { HttpContext = httpContext, RequestHandlerResolver = this });
        }

        protected void OnResolveHandlerComplete(IHttpContext httpContext, ResponderContextHandler<TResponder> requestHandler)
        {
            ResolveHandlerComplete?.Invoke(this, new RequestHandlerResolverEventArgs<TResponder> { HttpContext = httpContext, RequestHandlerResolver = this, RequestHandler = requestHandler });
        }

        protected void OnResolveHandlerExceptionThrown(IHttpContext httpContext, Exception ex)
        {
            ResolveHandlerExceptionThrown?.Invoke(this, new RequestHandlerResolverEventArgs<TResponder> { HttpContext = httpContext, RequestHandlerResolver = this, Exception = ex });
        }


    }
}
