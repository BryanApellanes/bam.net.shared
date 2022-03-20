using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IHttpRequestEncryptor
    {
        IEncryptor ContentEncryptor { get;  }
        IHttpRequestHeaderEncryptor HeaderEncryptor { get; }

        IEncryptedHttpRequest EncryptRequest(IHttpRequest request);
    }
}
