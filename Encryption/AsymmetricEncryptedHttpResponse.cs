using Bam.Net.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AsymmetricEncryptedHttpResponse<T> : EncryptedHttpResponse
    {
        public AsymmetricEncryptedHttpResponse()
        {
            this.StatusCode = 200;
        }

        public AsymmetricEncryptedHttpResponse(T data, IRsaPublicKeySource rsaPublicKeySource) :this()
        {
            AsymmetricDataEncryptor<T> encryptor = new AsymmetricDataEncryptor<T>(rsaPublicKeySource);
            this.ContentCipher = new AsymmetricContentCipher(encryptor.Encrypt(data));
        }
    }
}
