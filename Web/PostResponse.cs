using Bam.Net.Analytics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Web
{
    public class PostResponse : HttpClientResponse
    {
        public static implicit operator string(PostResponse response)
        {
            return response.Content;
        }

        public PostResponse()
        {
            Headers = new Dictionary<string, string>();
        }
    }
}
