using Bam.Net.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AsymmetricEncryptedHttpResponse<T> : EncryptedHttpResponse
    {
        public AsymmetricEncryptedHttpResponse(T data, IRsaPublicKeySource rsaPublicKeySource)
        {
            AsymmetricDataEncryptor<T> encryptor = new AsymmetricDataEncryptor<T>(rsaPublicKeySource);
            this.ContentCipher = new AsymmetricContentCipher(encryptor.Encrypt(data));
        }
    }
}
