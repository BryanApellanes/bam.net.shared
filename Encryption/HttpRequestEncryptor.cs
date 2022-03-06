using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequestEncryptor : IHttpRequestEncryptor
    {
        public HttpRequestEncryptor(IEncryptor encryptor)
        {
            this.Encryptor = encryptor;
            this.HeaderEncryptor = new HttpRequestHeaderEncryptor(encryptor);
        }

        public HttpRequestEncryptor(IEncryptor encryptor, IHttpRequestHeaderEncryptor headerEncryptor)
        {
            this.Encryptor = encryptor;
            this.HeaderEncryptor = headerEncryptor;
        }

        public IEncryptor Encryptor
        {
            get;
            private set;
        }

        public IHttpRequestHeaderEncryptor HeaderEncryptor
        {
            get;
            set;
        }

        public IHttpRequest EncryptRequest(IHttpRequest request)
        {
            EncryptedHttpRequest copy = new EncryptedHttpRequest();
            copy.Copy(request);
            copy.ContentCipher = Encryptor.EncryptString(request.Content);
            HeaderEncryptor.EncryptHeaders(copy);
            return copy;
        }
    }
}
