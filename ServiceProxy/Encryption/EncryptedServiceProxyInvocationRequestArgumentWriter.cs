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
                { EncryptionSchemes.Invalid, MediaTypes.SymmetricCipher },
                { EncryptionSchemes.Symmetric, MediaTypes.SymmetricCipher },
                { EncryptionSchemes.Asymmetric, MediaTypes.AsymmetricCipher },
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
            WriteEncryptedArgumentContent(requestMessage, secureChannelRequestMessage);
        }

        internal EncryptedServiceProxyInvocationHttpRequestContext WriteEncryptedArgumentContent(HttpRequestMessage requestMessage, SecureChannelRequestMessage secureChannelRequestMessage)
        {
            IEncryptor<SecureChannelRequestMessage> encryptor = GetEncryptor();
            requestMessage.Content = new ByteArrayContent(encryptor.Encrypt(secureChannelRequestMessage));
            MediaTypeHeaderValue contentType = GetContentType();
            requestMessage.Content.Headers.ContentType = contentType;
            
            ValueTransformerPipeline<SecureChannelRequestMessage> transformerPipeline = (ValueTransformerPipeline<SecureChannelRequestMessage>)encryptor;
            return new EncryptedServiceProxyInvocationHttpRequestContext
            {
                Encryptor = transformerPipeline,
                PlainPostBody = transformerPipeline.ConvertDataToString(secureChannelRequestMessage),
                HttpRequestMessage = requestMessage,
                ContentType = contentType.MediaType,
                ServiceType = this.ServiceType,
                MethodInfo = this.MethodInfo,
            };
        }

        protected SymmetricDataEncryptor<SecureChannelRequestMessage> GetSymmetricEncryptor()
        {
            return new SymmetricDataEncryptor<SecureChannelRequestMessage>(ClientSessionInfo);
        }

        protected AsymmetricDataEncryptor<SecureChannelRequestMessage> GetAsymmetricEncryptor()
        {
            return new AsymmetricDataEncryptor<SecureChannelRequestMessage>(ClientSessionInfo);
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

            IEncryptor<SecureChannelRequestMessage> encryptor = null;
            if (encryptAttribute != null)
            {
                switch (encryptAttribute.EncryptionScheme)
                {
                    case EncryptionSchemes.Asymmetric:
                        encryptor = GetAsymmetricEncryptor();
                        break;
                    case EncryptionSchemes.Invalid:
                    case EncryptionSchemes.Symmetric:
                    default:
                        encryptor = GetSymmetricEncryptor();
                        break;
                }
            }

            if (encryptor == null)
            {
                encryptor = GetSymmetricEncryptor();
            }

            return encryptor;
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

            return new MediaTypeHeaderValue(MediaTypes.SymmetricCipher);
        }
    }
}
