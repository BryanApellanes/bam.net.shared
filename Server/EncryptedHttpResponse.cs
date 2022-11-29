using Bam.Net.Encryption;
using Bam.Net.ServiceProxy.Encryption;
using Bam.NET.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public abstract class EncryptedHttpResponse : HttpResponse
    {

        ContentCipher _conentCipher;
        public ContentCipher ContentCipher
        {
            get {  return _conentCipher; }
            set
            {
                _conentCipher = value;
                Content = _conentCipher;
                ContentType = _conentCipher.ContentType;
            }
        }

        public static EncryptedHttpResponse ForData<T>(T data, IClientKeySource clientSessionInfo, EncryptionSchemes encryptionScheme)
        {
            switch (encryptionScheme)
            {
                case EncryptionSchemes.Invalid:
                case EncryptionSchemes.Symmetric:
                default:
                    return CreateSymmetricResponseForData(data, clientSessionInfo);
                case EncryptionSchemes.Asymmetric:
                    return CreateAsymmetricResponseForData(data, clientSessionInfo);
            }
        }

        public static EncryptedHttpResponse CreateAsymmetricResponseForData<T>(T data, IClientKeySource clientKeySource)
        {
            return new AsymmetricEncryptedHttpResponse<T>(data, clientKeySource);
        }

        public static EncryptedHttpResponse CreateSymmetricResponseForData<T>(T data, IClientKeySource clientKeySource)
        {
            return new SymmetricEncryptedHttpResponse<T>(data, clientKeySource);
        }
    }
}
