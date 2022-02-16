using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class SecureChannelRequestMessageSymmetricEncryptor : ValueTransformerPipeline<SecureChannelRequestMessage>
    {
        public SecureChannelRequestMessageSymmetricEncryptor(IAesKeySource aesKeySource)
        {
            this.AesByteTransformer = new AesByteTransformer(aesKeySource);
            this.GZipByteTransformer = new GZipByteTransformer();

            this.Add(this.AesByteTransformer);
            this.Add(this.GZipByteTransformer);
        }

        protected AesByteTransformer AesByteTransformer { get; private set; }
        protected GZipByteTransformer GZipByteTransformer { get; private set; }
    }
}
