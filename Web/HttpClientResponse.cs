using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Web
{
    public abstract class HttpClientResponse : IHttpClientResponse
    {
        public static implicit operator string(HttpClientResponse response)
        {
            return response.Content;
        }

        static HttpClientResponse()
        {
            Empty = new EmptyHttpClientResponse();
        }

        public Dictionary<string, string> Headers { get; protected set; }
        public virtual string Content { get; set; }
        public int StatusCode { get; set; }
        public string ContentType { get; set; }

        public Uri Url { get; set; }

        public static HttpClientResponse Empty
        {
            get;
            private set;
        }
            
    }
}
