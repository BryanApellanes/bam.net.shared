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

        public override TData ReverseTransform(byte[] output)
        {
            return GetReverseTransformer().ReverseTransform(output); 
        }

        public override byte[] Transform(TData input)
        {
            string json = input.ToJson();
            return Encoding.GetBytes(json);
        }

        public override IValueReverseTransformer<byte[], TData> GetReverseTransformer()
        {
            return new JsonByteReverseTransformer<TData>();
        }
    }
}
