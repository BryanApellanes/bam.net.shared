using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequestEncryptor<TContent> : HttpRequestEncryptor, IHttpRequestEncryptor<TContent>
    {
        public HttpRequestEncryptor(IEncryptor<TContent> encryptor):base(encryptor)
        {
            this.ContentEncryptor = encryptor;
        }

        public HttpRequestEncryptor(IEncryptor<TContent> contentEncryptor, IEncryptor headerEncryptor) : base(contentEncryptor, headerEncryptor)
        {
            this.ContentEncryptor = contentEncryptor;
        }

        public new IEncryptor<TContent> ContentEncryptor { get; private set; }

        public IHttpRequest<TContent> EncryptRequest(IHttpRequest<TContent> request)
        {
            EncryptedHttpRequest<TContent> copy = new EncryptedHttpRequest<TContent>();
            copy.Copy(request);
            copy.ContentCipher = ContentEncryptor.Encrypt(request.Content);
            HeaderEncryptor.EncryptHeaders(copy);
            return copy;
        }
    }
}
