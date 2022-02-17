using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class SecureChannelRequestMessageSymmetricDecryptor : ValueReverseTransformerPipeline<SecureChannelRequestMessage>, IDecryptor<SecureChannelRequestMessage>
    {
        public SecureChannelRequestMessageSymmetricDecryptor(SecureChannelRequestMessageSymmetricEncryptor tranformerPipeline) : base(tranformerPipeline)
        {
        }

        public SecureChannelRequestMessage Decrypt(byte[] cipherData)
        {
            return ReverseTransform(cipherData);
        }
    }
}
