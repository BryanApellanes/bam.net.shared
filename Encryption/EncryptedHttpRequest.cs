using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class EncryptedHttpRequest : HttpRequest
    {
        public Cipher ContentCipher { get; internal set; }
    }
}
