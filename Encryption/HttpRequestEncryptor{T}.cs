using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequestEncryptor<TContent> : HttpRequestEncryptor, IHttpRequestEncryptor<TContent>
    {
        public HttpRequestEncryptor(IEncryptor<TContent> encryptor):base(encryptor)
        {
            this.Encryptor = encryptor;
        }

        public HttpRequestEncryptor(IAesKeySource aesKeySource): this(new SymmetricEncryptor<TContent>(aesKeySource))
        {
        }

        public HttpRequestEncryptor(IRsaPublicKeySource rsaPublicKeySource) : this(new AsymmetricEncryptor<TContent>(rsaPublicKeySource))
        { 
        }

        public new IEncryptor<TContent> Encryptor { get; private set; }

        public IHttpRequest<TContent> EncryptRequest(IHttpRequest<TContent> request)
        {
            EncryptedHttpRequest<TContent> copy = new EncryptedHttpRequest<TContent>();
            copy.Copy(request);
            copy.ContentCipher = Encryptor.Encrypt(request.Content);
            HeaderEncryptor.EncryptHeaders(copy);
            return copy;
        }
    }
}
