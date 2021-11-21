using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Bam.Net.Server
{
    public class HttpMethodHandlers
    {
        public HttpMethodHandlers()
        {
            this.HandlerFunctions = new Dictionary<HttpMethod, Func<IRequest, IHttpResponse>>();
        }

        public void SetHandler(string httpMethod, Func<IRequest, IHttpResponse> handlerFunction)
        {
            SetHandler(new HttpMethod(httpMethod), handlerFunction);
        }

        public void SetHandler(HttpMethod httpMethod, Func<IRequest, IHttpResponse> handlerFunction)
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

        public IHttpResponse HandleRequest(IRequest request)
        {
            HttpMethod httpMethod = new HttpMethod(request.HttpMethod);
            if (HandlerFunctions.ContainsKey(httpMethod))
            {
                return HandlerFunctions[httpMethod](request);
            }
            return new HttpResponse("Not Found", 404);
        }

        protected Dictionary<HttpMethod, Func<IRequest, IHttpResponse>> HandlerFunctions { get; }
    }
}
