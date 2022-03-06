using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequest : IHttpRequest
    {
        public IDictionary<string, string> Headers
        {
            get;
            private set;
        }

        public string ContentType
        {
            get;
            set;
        }

        public HttpVerbs Verb
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public void Copy(IHttpRequest request)
        {
            this.Content = request.Content;
            this.ContentType = request.ContentType;
            this.Verb = request.Verb;
            foreach (string key in request.Headers.Keys)
            {
                this.Headers.Add(key, request.Headers[key]);
            }
        }
    }
}
