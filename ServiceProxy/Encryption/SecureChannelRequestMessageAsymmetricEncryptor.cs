using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class SecureChannelRequestMessageAsymmetricEncryptor : ValueTransformerPipeline<SecureChannelRequestMessage>, IEncryptor<SecureChannelRequestMessage>
    {
        public SecureChannelRequestMessageAsymmetricEncryptor(IRsaPublicKeySource rsaPublicKeySource)
        {
            this.RsaByteTransformer = new RsaByteTransformer(rsaPublicKeySource);
            this.GZipByteTransformer = new GZipByteTransformer();

            this.Add(this.RsaByteTransformer);
            this.Add(this.GZipByteTransformer);
        }

        public SecureChannelRequestMessageAsymmetricEncryptor(RsaPublicPrivateKeyPair rsaPublicPrivateKeyPair)
        {
            this.RsaByteTransformer = new RsaByteTransformer(rsaPublicPrivateKeyPair);
            this.GZipByteTransformer = new GZipByteTransformer();

            this.Add(this.RsaByteTransformer);
            this.Add(this.GZipByteTransformer);
        }

        protected RsaByteTransformer RsaByteTransformer { get; private set; }
        protected GZipByteTransformer GZipByteTransformer { get; private set; }
        
        public new SecureChannelRequestMessageAsymmetricDecryptor GetReverseTransformer()
        {
            return new SecureChannelRequestMessageAsymmetricDecryptor(this);
        }

        public byte[] Encrypt(SecureChannelRequestMessage data)
        {
            return Transform(data);
        }

        public IDecryptor<SecureChannelRequestMessage> GetDecryptor()
        {
            return GetReverseTransformer();
        }
    }
}
