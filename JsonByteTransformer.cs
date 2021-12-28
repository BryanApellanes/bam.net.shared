using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class JsonByteTransformer<TData> : ValueTransformer<TData, byte[]>
    {
        public JsonByteTransformer()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public override TData Untransform(byte[] output)
        {
            return GetUntransformer().Untransform(output); 
        }

        public override byte[] Transform(TData input)
        {
            string json = input.ToJson();
            return Encoding.GetBytes(json);
        }

        public override IValueUntransformer<byte[], TData> GetUntransformer()
        {
            return new JsonByteUntransformer<TData>();
        }
    }
}
