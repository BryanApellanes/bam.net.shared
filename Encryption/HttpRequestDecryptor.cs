using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequestDecryptor : IHttpRequestDecryptor
    {
        public HttpRequestDecryptor(IDecryptor decryptor)
        {
            this.ContentDecryptor = decryptor;
            this.HeaderDecryptor = new HttpRequestHeaderDecryptor(decryptor);
        }

        public HttpRequestDecryptor(IDecryptor contentDecryptor, IDecryptor headerDecryptor)
        {
            this.ContentDecryptor = contentDecryptor;
            this.HeaderDecryptor = new HttpRequestHeaderDecryptor(headerDecryptor);
        }

        public IDecryptor ContentDecryptor
        {
            get;
            private set;
        }

        public IHttpRequestHeaderDecryptor HeaderDecryptor
        {
            get;
            private set;
        }

        public IHttpRequest DecryptRequest(IEncryptedHttpRequest request)
        {
            HttpRequest copy = new HttpRequest();
            copy.Copy(request);
            copy.Content = ContentDecryptor.DecryptString(request.ContentCipher);
            HeaderDecryptor.DecryptHeaders(copy);
            return copy;
        }
    }
}
