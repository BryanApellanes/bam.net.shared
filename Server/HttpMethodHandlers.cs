using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Bam.Net.Server
{
    /// <summary>
    /// A collection of handlers keyed by HttpMethod.
    /// </summary>
    public class HttpMethodHandlers
    {
        public HttpMethodHandlers()
        {
            this.HandlerFunctions = new Dictionary<HttpMethod, Func<IHttpContext, IHttpResponse>>();
        }

        public void SetHandler(string httpMethod, Func<IHttpContext, IHttpResponse> handlerFunction)
        {
            SetHandler(new HttpMethod(httpMethod), handlerFunction);
        }

        public void SetHandler(HttpMethod httpMethod, Func<IHttpContext, IHttpResponse> handlerFunction)
        {
            if (HandlerFunctions.ContainsKey(httpMethod))
            {
                HandlerFunctions[httpMethod] = handlerFunction;
            }
            else
            {
                HandlerFunctions.Add(httpMethod, handlerFunction);
            }
        }

        public IHttpResponse HandleRequest(IHttpContext context)
        {
            HttpMethod httpMethod = new HttpMethod(context.Request.HttpMethod);
            if (HandlerFunctions.ContainsKey(httpMethod))
            {
                return HandlerFunctions[httpMethod](context);
            }
            return new HttpResponse("Not Found", 404);
        }

        protected Dictionary<HttpMethod, Func<IHttpContext, IHttpResponse>> HandlerFunctions { get; }
    }
}
