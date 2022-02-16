using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        public override void WriteArguments(HttpRequestMessage requestMessage)
        {
            // use a ByteTransformerPipeline internally 
            // use ByteArrayContent
            base.WriteArguments(requestMessage); 
        }

        protected SecureChannelRequestMessageSymmetricEncryptor GetSymmetricEncryptor()
        {
            throw new NotImplementedException();
        }

        protected ValueTransformerPipeline<SecureChannelRequestMessage> AsymmetricValueTransformer
        {
            get;
            private set;
        }

        protected Dictionary<EncryptionSchemes, string> EncryptionSchemesToContentTypes
        {
            get;
            private set;
        }

        protected string GetContentType()
        {
            EncryptAttribute encryptAttribute = this.ServiceType.GetCustomAttributeOfType<EncryptAttribute>(); ;
            if (encryptAttribute == null)
            {
                encryptAttribute = this.MethodInfo.GetCustomAttributeOfType<EncryptAttribute>();
            }

            if (encryptAttribute != null)
            {
                return EncryptionSchemesToContentTypes[encryptAttribute.EncryptionScheme];
            }
            
            return ContentTypes.SymmetricCipher;
        }
    }
}
