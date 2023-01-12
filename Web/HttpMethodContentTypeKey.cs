using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Web
{
    public class HttpMethodContentTypeKey
    {
        public HttpMethodContentTypeKey(string httpVerb)
        {
            HttpMethod = httpVerb.ToUpperInvariant();
        }

        public HttpMethodContentTypeKey(string httpVerb, string contentType) : this(httpVerb)
        {
            if (!string.IsNullOrEmpty(contentType) && contentType.Contains("+"))
            {
                ContentType = contentType.ReadUntil('+');
            }
            else
            {
                ContentType = contentType;
            }
        }

        public HttpMethodContentTypeKey(IRequest request) : this(request.HttpMethod, request.ContentType)
        {
        }

        public string HttpMethod { get; }
        public string ContentType { get; }

        public override bool Equals(object obj)
        {
            if (obj is HttpMethodContentTypeKey value)
            {
                return value.ToString().Equals(ToString());
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{HttpMethod}:{ContentType}";
        }
    }
}
