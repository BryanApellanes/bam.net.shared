using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server.ServiceProxy
{
    public class ContentTypeMethod
    {
        public ContentTypeMethod(string httpMethod)
        {
            this.HttpMethod = httpMethod.ToUpperInvariant();
        }

        public ContentTypeMethod(string httpMethod, string contentType): this(httpMethod)
        {
            this.ContentType = contentType;
        }

        public ContentTypeMethod(IRequest request):this(request.HttpMethod, request.ContentType)
        {

        }

        public string HttpMethod { get; }
        public string ContentType { get; }

        public override bool Equals(object obj)
        {
            if(obj is ContentTypeMethod value)
            {
                return value.ToString().Equals(this.ToString());
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{HttpMethod}:{ContentType}";
        }
    }
}
