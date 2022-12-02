using Microsoft.CodeAnalysis.CSharp.Syntax;
using MongoDB.Driver.Core.WireProtocol.Messages.Encoders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Web
{
    public interface IHttpClientResponse
    {
        Dictionary<string, string> Headers { get; }

        string Content { get; }
        int StatusCode { get; }
        string ContentType { get; }

        Uri Url { get; }
    }
}
