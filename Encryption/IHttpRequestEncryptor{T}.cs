using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    /// <summary>
    /// A class used to encrypt the content body of a request.
    /// </summary>
    /// <typeparam name="TContent"></typeparam>
    public interface IHttpRequestEncryptor<TContent> : IHttpRequestEncryptor
    {
        new IContentEncryptor<TContent> ContentEncryptor { get; }

        /// <summary>
        /// Returns an encrypted copy of the specified request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        EncryptedHttpRequest<TContent> EncryptRequest(IHttpRequest<TContent> request);
    }
}
