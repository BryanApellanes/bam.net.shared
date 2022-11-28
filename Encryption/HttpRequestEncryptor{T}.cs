using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequestEncryptor<TContent> : HttpRequestEncryptor, IHttpRequestEncryptor<TContent>
    {
        public HttpRequestEncryptor(IContentEncryptor<TContent> encryptor) : base(encryptor)
        {
            this.ContentEncryptor = encryptor;
        }

        public HttpRequestEncryptor(IContentEncryptor<TContent> contentEncryptor, IEncryptor headerEncryptor) : base(contentEncryptor, headerEncryptor)
        {
            this.ContentEncryptor = contentEncryptor;
        }

        public new IContentEncryptor<TContent> ContentEncryptor { get; private set; }

        /// <summary>
        /// Returns an encrypted copy of the specified request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EncryptedHttpRequest<TContent> EncryptRequest(IHttpRequest<TContent> request)
        {
            EncryptedHttpRequest<TContent> copy = new EncryptedHttpRequest<TContent>();
            copy.Copy(request);
            ContentCipher<TContent> cipher = ContentEncryptor.GetContentCipher(request.TypedContent);
            HeaderEncryptor.EncryptHeaders(copy);
            copy.ContentCipher = cipher;
            copy.ContentType = cipher.ContentType;
            return copy;
        }
    }
}
