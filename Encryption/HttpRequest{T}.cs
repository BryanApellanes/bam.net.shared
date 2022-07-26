using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequest<TContent> : HttpRequest, IHttpRequest<TContent>
    {
        TContent content;

        public HttpRequest():base()
        {
        }

        public new TContent Content
        {
            get
            {
                if (this.content == null && !string.IsNullOrEmpty(base.Content))
                {
                    base.Content.TryFromJson<TContent>(out this.content);
                }
                return this.content;
            }
            set
            {
                this.content = value;
                base.Content = this.Content.ToJson();
            }
        }

        public void Copy(IHttpRequest<TContent> request)
        {
            this.Uri = request.Uri;
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
