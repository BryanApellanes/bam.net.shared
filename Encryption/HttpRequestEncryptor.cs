using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequestEncryptor : IHttpRequestEncryptor
    {
        public HttpRequestEncryptor(IEncryptor encryptor)
        {
            this.ContentEncryptor = encryptor;
            this.HeaderEncryptor = new HttpRequestHeaderEncryptor(encryptor);
        }

        public HttpRequestEncryptor(IEncryptor contentEncryptor, IEncryptor headerEncryptor)
        {
            this.ContentEncryptor = contentEncryptor;
            this.HeaderEncryptor = new HttpRequestHeaderEncryptor(headerEncryptor);
        }

        public IEncryptor ContentEncryptor
        {
            get;
            private set;
        }

        public IHttpRequestHeaderEncryptor HeaderEncryptor
        {
            get;
            private set;
        }

        public EncryptedHttpRequest EncryptRequest(IHttpRequest request)
        {
            EncryptedHttpRequest copy = new EncryptedHttpRequest();
            copy.Copy(request);
            copy.ContentCipher = ContentEncryptor.EncryptString(request.Content);
            HeaderEncryptor.EncryptHeaders(copy);
            return copy;
        }
    }
}
