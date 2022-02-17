using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class SecureChannelRequestMessageAsymmetricDecryptor : ValueReverseTransformerPipeline<SecureChannelRequestMessage>, IDecryptor<SecureChannelRequestMessage>
    {
        public SecureChannelRequestMessageAsymmetricDecryptor(SecureChannelRequestMessageAsymmetricEncryptor tranformerPipeline) : base(tranformerPipeline)
        {
        }

        public SecureChannelRequestMessage Decrypt(byte[] cipherData)
        {
            return ReverseTransform(cipherData);
        }
    }
}
