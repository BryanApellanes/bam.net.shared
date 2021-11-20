using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    internal class ResponderRequestHandlerResolver : RequestHandlerResolver
    {
        Dictionary<string, RequestHandler> _pathRequestHandlers;
        RequestHandler _defaultRequestHandler;
        public ResponderRequestHandlerResolver(Responder responder)
        {
            this._pathRequestHandlers = new Dictionary<string, RequestHandler>();
            this.Responder = responder;
        }

        public Responder Responder { get; set; }

        public void AddPathNameHandler(string pathName, RequestHandler requestHandler, bool isDefault = false)
        {
            _pathRequestHandlers.Add(pathName.ToLowerInvariant(), requestHandler);
            if (isDefault)
            {
                _defaultRequestHandler = requestHandler;
            }
        }

        protected override RequestHandler ResolveHandler(IRequest request)
        {
            Args.ThrowIfNull(request, nameof(request));

            PathNameRoute responderPathRoute = PathNameRoute.FromRequest(request);

            if(responderPathRoute.PathName.Equals(Responder.ResponderName))
            {
                PathNameRoute requestHandlerRoute = PathNameRoute.FromPath(responderPathRoute.Path);
                string key = requestHandlerRoute.PathName.ToLowerInvariant();
                if (_pathRequestHandlers.ContainsKey(key))
                {
                    return _pathRequestHandlers[key];
                }
            }
            
            return _defaultRequestHandler;
        }
    }
}
