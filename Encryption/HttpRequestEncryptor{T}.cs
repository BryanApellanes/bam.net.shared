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

        /// <inheritdoc />
        public IEncryptedHttpRequest<TContent> EncryptRequest(IHttpRequest<TContent> request)
        {
            EncryptedHttpRequest<TContent> copy = new EncryptedHttpRequest<TContent>();
            copy.Copy(request);
            ContentCipher<TContent> cipher = ContentEncryptor.GetContentCipher(request.Content);
            copy.ContentCipher = cipher;
            copy.Headers.Add("Content-Type", cipher.ContentType);
            HeaderEncryptor.EncryptHeaders(copy);
            return copy;
        }
    }
}
