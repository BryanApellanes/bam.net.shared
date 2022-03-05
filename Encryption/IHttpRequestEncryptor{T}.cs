using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IHttpRequestEncryptor<TContent> : IHttpRequestEncryptor
    {
        new IEncryptor<TContent> Encryptor { get; }

        IHttpRequest<TContent> Encrypt(IHttpRequest<TContent> request);
    }
}
