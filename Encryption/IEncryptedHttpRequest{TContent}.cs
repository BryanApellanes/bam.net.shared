using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IEncryptedHttpRequest<TContent> : IEncryptedHttpRequest, IHttpRequest<TContent>
    {
        new ContentCipher<TContent> ContentCipher { get; }
    }
}
