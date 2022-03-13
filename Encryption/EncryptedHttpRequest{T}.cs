using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class EncryptedHttpRequest<TContent> : HttpRequest<TContent>
    {
        public ContentCipher<TContent> ContentCipher { get; internal set; }
    }
}
