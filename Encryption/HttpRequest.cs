using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequest : IHttpRequest
    {
        public HttpRequest()
        {
            this.Uri = new Uri("https://localhost");
            this.Encoding = Encoding.UTF8;
            this.Headers = new Dictionary<string, string>();
            this.ContentType = MediaTypes.Json;
        }

        public Uri Uri
        {
            get;
            set;
        }

        public IDictionary<string, string> Headers
        {
            get;
            private set;
        }

        public virtual string ContentType
        {
            get;
            set;
        }

        public HttpVerbs Verb
        {
            get;
            set;
        }

        public virtual string Content
        {
            get;
            set;
        }

        public Encoding Encoding
        {
            get;
            set;
        }

        public virtual void Copy(IHttpRequest request)
        {
            this.Uri = request.Uri;
            this.Content = request.Content;
            this.ContentType = request.ContentType;
            this.Verb = request.Verb;
            foreach (string key in request.Headers.Keys)
            {
                this.Headers.Add(key, request.Headers[key]);
            }
        }

        public HttpRequestMessage ToHttpRequestMessage(string url)
        {
            HttpRequestMessage requestMessage = ToHttpRequestMessage();
            requestMessage.RequestUri = new Uri(url);
            return requestMessage;
        }

        public virtual HttpRequestMessage ToHttpRequestMessage()
        {
            HttpRequestMessage result = new HttpRequestMessage(MethodsByVerbs[Verb], Uri)
            {
                Content = new StringContent(Content, Encoding, ContentType)
            };
            foreach(string key in Headers.Keys)
            {
                result.Headers.Add(key, Headers[key]);
            }
            return result;
        }

        public static HttpRequest FromHttpRequestMessage(HttpRequestMessage httpRequestMessage)
        {
            HttpRequest request = new HttpRequest
            {
                Uri = httpRequestMessage.RequestUri,
                Content = httpRequestMessage.Content.ReadAsStringAsync().Result,
                ContentType = httpRequestMessage.Content?.Headers?.ContentType?.MediaType,
                Verb = VerbsByMethod[httpRequestMessage.Method]
            };
            foreach(System.Collections.Generic.KeyValuePair<string, IEnumerable<string>> kvp in httpRequestMessage.Headers)
            {
                request.Headers.Add(kvp.Key, kvp.Value.ToString());
            }
            return request;
        }

        public static HttpRequest<TContent> FromHttpRequestMessage<TContent>(HttpRequestMessage httpRequestMessage)
        {
            HttpRequest<TContent> request = new HttpRequest<TContent>
            {
                Uri = httpRequestMessage.RequestUri,
                ContentType = httpRequestMessage.Content?.Headers?.ContentType?.MediaType,
                Verb = VerbsByMethod[httpRequestMessage.Method]
            };

            if (httpRequestMessage.Content != null && 
                httpRequestMessage.Content.ReadAsStringAsync().Result.TryFromJson<TContent>(out TContent content))
            {
                request.TypedContent = content;
            }
            foreach (System.Collections.Generic.KeyValuePair<string, IEnumerable<string>> kvp in httpRequestMessage.Headers)
            {
                request.Headers.Add(kvp.Key, kvp.Value.ToArray().ToDelimited(val => val, ","));
            }
            return request;
        }

        static Dictionary<HttpMethod, HttpVerbs> _verbsByMethod;
        protected static Dictionary<HttpMethod, HttpVerbs> VerbsByMethod
        {
            get
            {
                if (_verbsByMethod == null)
                {
                    _verbsByMethod = new Dictionary<HttpMethod, HttpVerbs>
                    {
                        { HttpMethod.Get, HttpVerbs.Get },
                        { HttpMethod.Post, HttpVerbs.Post },
                        { HttpMethod.Put, HttpVerbs.Put },
                        { HttpMethod.Delete, HttpVerbs.Delete },
                        { HttpMethod.Head, HttpVerbs.Head },
                        { HttpMethod.Options, HttpVerbs.Options },
                        { HttpMethod.Trace, HttpVerbs.Trace }
                    };
                }
                return _verbsByMethod;
            }
        }

        Dictionary<HttpVerbs, HttpMethod> _methodsByVerbs;
        private Dictionary<HttpVerbs, HttpMethod> MethodsByVerbs
        {
            get
            {
                if(_methodsByVerbs == null)
                {
                    _methodsByVerbs = new Dictionary<HttpVerbs, HttpMethod> 
                    {
                        { HttpVerbs.Get, HttpMethod.Get },
                        { HttpVerbs.Post, HttpMethod.Post },
                        { HttpVerbs.Put, HttpMethod.Put },
                        { HttpVerbs.Delete, HttpMethod.Delete },
                        { HttpVerbs.Head, HttpMethod.Head },
                        { HttpVerbs.Options, HttpMethod.Options },
                        { HttpVerbs.Trace, HttpMethod.Trace }
                    };
                }
                return _methodsByVerbs;
            }
        }
    }
}
