using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequest<TContent> : IHttpRequest<TContent>
    {
        public HttpRequest()
        {
            this.Headers = new Dictionary<string, string>();
        }

        public TContent Content
        {
            get; 
            set;
        }

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

        string IHttpRequest.Content
        {
            get
            {
                return this.Content?.ToJson();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.Content = value.FromJson<TContent>();
                }
            }
        }

        public void Copy(IHttpRequest<TContent> request)
        {
            this.Content = request.Content;
            this.ContentType = request.ContentType;
            this.Verb =  request.Verb;
            foreach (string key in request.Headers.Keys)
            {
                this.Headers.Add(key, request.Headers[key]);
            }
        }
    }
}
