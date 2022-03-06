using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IHttpRequest
    {
        IDictionary<string, string> Headers { get; }
        string ContentType { get; set; }
        HttpVerbs Verb { get; set; }
        string Content { get; set; }
    }
}
