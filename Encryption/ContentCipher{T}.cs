using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public abstract class ContentCipher<TContent> : Cipher<TContent>
    {
        public string ContentType { get; protected set; }
    }
}
