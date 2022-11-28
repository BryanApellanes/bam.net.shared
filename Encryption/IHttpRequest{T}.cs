using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IHttpRequest<TContent> : IHttpRequest
    {
        new TContent TypedContent { get; set; }
    }
}
