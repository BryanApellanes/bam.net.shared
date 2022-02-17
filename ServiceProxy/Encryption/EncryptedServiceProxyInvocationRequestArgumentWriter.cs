using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class EncryptedServiceProxyInvocationRequestArgumentWriter : ServiceProxyInvocationRequestArgumentWriter
    {
        public EncryptedServiceProxyInvocationRequestArgumentWriter(ServiceProxyInvocationRequest invocationRequest, IApiArgumentEncoder apiArgumentEncoder = null) : base(invocationRequest, apiArgumentEncoder)
        {
            this.EncryptionSchemesToContentTypes = new Dictionary<EncryptionSchemes, string>()
            {
                { EncryptionSchemes.Invalid, ContentTypes.SymmetricCipher },
                { EncryptionSchemes.Symmetric, ContentTypes.SymmetricCipher },
                { EncryptionSchemes.Asymmetric, ContentTypes.AsymmetricCipher },
            };
        }

        public ClientSessionInfo ClientSessionInfo { get; set; }

        public override void WriteArguments(HttpRequestMessage requestMessage)
        {
            this.WriteArgumentContent(requestMessage);
        }

        public override void WriteArgumentContent(HttpRequestMessage requestMessage)
        {
            SecureChannelRequestMessage secureChannelRequestMessage = new SecureChannelRequestMessage(this.ServiceProxyInvocationRequest);
            IEncryptor<SecureChannelRequestMessage> encryptor = GetEncryptor();
            requestMessage.Content = new ByteArrayContent(encryptor.Encrypt(secureChannelRequestMessage));
            requestMessage.Content.Headers.ContentType = GetContentType();
        }

        protected SecureChannelRequestMessageSymmetricEncryptor GetSymmetricEncryptor()
        {
            return new SecureChannelRequestMessageSymmetricEncryptor(ClientSessionInfo);
        }

        protected SecureChannelRequestMessageAsymmetricEncryptor GetAsymmetricEncryptor()
        {
            return new SecureChannelRequestMessageAsymmetricEncryptor(ClientSessionInfo);
        }

        protected Dictionary<EncryptionSchemes, string> EncryptionSchemesToContentTypes
        {
            get;
            private set;
        }

        protected IEncryptor<SecureChannelRequestMessage> GetEncryptor()
        {
            EncryptAttribute encryptAttribute = this.ServiceType.GetCustomAttributeOfType<EncryptAttribute>(); ;
            if (encryptAttribute == null)
            {
                encryptAttribute = this.MethodInfo.GetCustomAttributeOfType<EncryptAttribute>();
            }

            if (encryptAttribute != null)
            {
                switch (encryptAttribute.EncryptionScheme)
                {
                    case EncryptionSchemes.Invalid:
                    case EncryptionSchemes.Symmetric:
                        return GetSymmetricEncryptor();
                    case EncryptionSchemes.Asymmetric:
                    default:
                        return GetAsymmetricEncryptor();
                }
            }

            return GetSymmetricEncryptor();
        }

        protected MediaTypeHeaderValue GetContentType()
        {
            EncryptAttribute encryptAttribute = this.ServiceType.GetCustomAttributeOfType<EncryptAttribute>(); ;
            if (encryptAttribute == null)
            {
                encryptAttribute = this.MethodInfo.GetCustomAttributeOfType<EncryptAttribute>();
            }

            if (encryptAttribute != null)
            {
                return new MediaTypeHeaderValue(EncryptionSchemesToContentTypes[encryptAttribute.EncryptionScheme]);
            }

            return new MediaTypeHeaderValue(ContentTypes.SymmetricCipher);
        }
    }
}
