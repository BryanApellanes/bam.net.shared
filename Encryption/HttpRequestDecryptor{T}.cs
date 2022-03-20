using bam.net.shared.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequestDecryptor<TContent> : HttpRequestDecryptor, IHttpRequestDecryptor<TContent>
    {
        public HttpRequestDecryptor(IContentDecryptor<TContent> decryptor) : base(decryptor)
        {
        }

        public HttpRequestDecryptor(IContentDecryptor<TContent> contentDecrpytor, IDecryptor headerDecryptor) : base(contentDecrpytor, headerDecryptor)
        {
            this.ContentDecryptor = contentDecrpytor;
        }

        public new IContentDecryptor<TContent> ContentDecryptor
        {
            get;
            private set;
        }

        public IHttpRequest<TContent> DecryptRequest(IEncryptedHttpRequest<TContent> request)
        {
            throw new NotImplementedException();
        }
    }
}
