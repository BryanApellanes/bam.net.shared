using Bam.Net.Encryption;
using Bam.Net.Server;
using Bam.Net.ServiceProxy.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.NET.Encryption
{
    public class SymmetricEncryptedHttpResponse<T> : EncryptedHttpResponse
    {
        public SymmetricEncryptedHttpResponse(T data, IAesKeySource aesKeySource)
        {
            SymmetricDataEncryptor<T> encryptor = new SymmetricDataEncryptor<T>(aesKeySource);
            this.ContentCipher = new SymmetricContentCipher(encryptor.Encrypt(data));
        }
    }
}
