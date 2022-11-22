using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public class RequestHandlerEventArgs<TResponder> : EventArgs
        where TResponder : Responder
    {        
        public IHttpContext HttpContext { get; set; }

        public IRequest Request
        {
            get => HttpContext?.Request;
        }

        public ResponderContextHandler<TResponder> RequestHandler { get; set; }

        public Exception Exception { get; set; }
    }
}
