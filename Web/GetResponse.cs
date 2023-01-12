using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Web
{
    public class GetResponse : HttpClientResponse
    {
        public static implicit operator string(GetResponse response)
        {
            return response.Content;
        }

        public GetResponse()
        {
            this.Headers = new Dictionary<string, string>();
        }
    }
}
