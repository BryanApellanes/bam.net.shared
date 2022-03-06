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
        new IEncryptor<TContent> Encryptor { get; }

        /// <summary>
        /// Encrypts the content of the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IHttpRequest<TContent> EncryptRequest(IHttpRequest<TContent> request);
    }
}
