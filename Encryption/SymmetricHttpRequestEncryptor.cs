using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class SymmetricHttpRequestEncryptor<TContent> : IHttpRequestEncryptor<TContent>
    {
        public SymmetricHttpRequestEncryptor(SymmetricEncryptor<TContent> encryptor)
        {
            this.Encryptor = encryptor;
        }

        public SymmetricHttpRequestEncryptor(IAesKeySource aesKeySource): this(new SymmetricEncryptor<TContent>(aesKeySource))
        {
        }

        public IEncryptor<TContent> Encryptor
        {
            get;
            private set;
        }

        IEncryptor IHttpRequestEncryptor.Encryptor => throw new NotImplementedException();

        public IHttpRequest<TContent> Encrypt(IHttpRequest<TContent> request)
        {
            throw new NotImplementedException();
        }

        public IHttpRequest Encrypt(IHttpRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
