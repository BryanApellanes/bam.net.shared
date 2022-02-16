using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class SecureChannelRequestMessageAsymmetricEncryptor : ValueTransformerPipeline<SecureChannelRequestMessage>
    {
        public SecureChannelRequestMessageAsymmetricEncryptor(IRsaPublicKeySource rsaPublicKeySource)
        {
            this.RsaByteTransformer = new RsaByteTransformer(rsaPublicKeySource);
            this.GZipByteTransformer = new GZipByteTransformer();

            this.Add(this.RsaByteTransformer);
            this.Add(this.GZipByteTransformer);
        }

        protected RsaByteTransformer RsaByteTransformer { get; private set; }
        protected GZipByteTransformer GZipByteTransformer { get; private set; }
    }
}
