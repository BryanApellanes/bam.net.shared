using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IHttpRequestDecryptor
    {
        IDecryptor ContentDecryptor { get; }

        IHttpRequestHeaderDecryptor HeaderDecryptor { get; }

        /// <summary>
        /// Decrypts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The uncrypted request.</returns>
        IHttpRequest DecryptRequest(IEncryptedHttpRequest request);
    }
}
