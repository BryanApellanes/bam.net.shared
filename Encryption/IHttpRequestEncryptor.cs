using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IHttpRequestEncryptor
    {
        IEncryptor Encryptor { get; }
        IHttpRequestHeaderEncryptor HeaderEncryptor { get; }

        IHttpRequest EncryptRequest(IHttpRequest request);
    }
}
