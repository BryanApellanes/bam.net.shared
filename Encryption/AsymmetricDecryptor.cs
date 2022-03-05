using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AsymmetricDecryptor<TData> : ValueReverseTransformerPipeline<TData>, IDecryptor<TData>
    {
        public AsymmetricDecryptor(AsymmetricEncryptor<TData> tranformerPipeline) : base(tranformerPipeline)
        {
        }

        public TData Decrypt(byte[] cipherData)
        {
            return ReverseTransform(cipherData);
        }
    }
}
