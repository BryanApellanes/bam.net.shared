using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IHttpRequestEncryptor
    {
        IEncryptor Encryptor { get; }
        IHttpRequest Encrypt(IHttpRequest request);
    }
}
