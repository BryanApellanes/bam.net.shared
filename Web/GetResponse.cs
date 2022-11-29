using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Web
{
    public class GetResponse : IGetResponse
    {
        public static implicit operator string(GetResponse response)
        {
            return response.Content;
        }

        public GetResponse()
        {
            this.Headers = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Headers { get; private set; }
        public string Content { get; set; }
        public int StatusCode { get; set; }
        public string ContentType { get; set; }

        public Uri Url { get; set; }
    }
}
