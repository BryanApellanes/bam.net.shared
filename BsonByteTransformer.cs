using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class BsonByteTransformer<TData> : ValueTransformer<TData, byte[]>
    {
        public override TData Untransform(byte[] output)
        {
            return GetReverseTransformer().ReverseTransform(output);
        }

        public override byte[] Transform(TData input)
        {
            return input.ToBson();
        }

        public override IValueReverseTransformer<byte[], TData> GetReverseTransformer()
        {
            return new BsonByteReverseTransformer<TData>();
        }
    }
}
