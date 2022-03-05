using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class SymmetricDecryptor<TData> : ValueReverseTransformerPipeline<TData>, IDecryptor<TData>
    {
        public SymmetricDecryptor(SymmetricEncryptor<TData> tranformerPipeline) : base(tranformerPipeline)
        {
        }

        public TData Decrypt(byte[] cipherData)
        {
            return ReverseTransform(cipherData);
        }
    }
}
