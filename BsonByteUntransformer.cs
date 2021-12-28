using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class BsonByteUntransformer<TData> : IValueUntransformer<byte[], TData>
    {
        public TData Untransform(byte[] encoded)
        {
            return encoded.FromBson<TData>();
        }

        public IValueTransformer<TData, byte[]> GetTransformer()
        {
            return new BsonByteTransformer<TData>();
        }
    }
}
