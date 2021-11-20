using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public class RequestHandlerEventArgs : EventArgs
    {        
        public IHttpContext HttpContext { get; set; }

        public IRequest Request
        {
            get => HttpContext?.Request;
        }

        public RequestHandler RequestHandler { get; set; }

        public Exception Exception { get; set; }
    }
}
