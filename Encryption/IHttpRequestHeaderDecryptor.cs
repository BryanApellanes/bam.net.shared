using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IHttpRequestHeaderDecryptor
    {
        IDecryptor Decryptor { get; }

        void DecryptHeaders(IHttpRequest request);
    }
}
