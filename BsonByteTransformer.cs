using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class BsonByteTransformer<TData> : ValueTransformer<TData, byte[]>
    {
        public override TData Untransform(byte[] output)
        {
            return GetUntransformer().Untransform(output);
        }

        public override byte[] Transform(TData input)
        {
            return input.ToBson();
        }

        public override IValueUntransformer<byte[], TData> GetUntransformer()
        {
            return new BsonByteUntransformer<TData>();
        }
    }
}
