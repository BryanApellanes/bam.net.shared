using Bam.Net.Analytics;
using Bam.Net.Web;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public class PostResponse : IPostResponse
    {
        public static implicit operator string(PostResponse response)
        {
            return response.Content;
        }

        public PostResponse() 
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
