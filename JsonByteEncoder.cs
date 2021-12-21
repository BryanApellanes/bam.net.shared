using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class JsonByteEncoder<TData> : ValueEncoder<TData, byte[]>
    {
        public JsonByteEncoder()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public override TData Decode(byte[] output)
        {
            return GetDecoder().Decode(output); 
        }

        public override byte[] Encode(TData input)
        {
            string json = input.ToJson();
            return Encoding.GetBytes(json);
        }

        public override IValueDecoder<byte[], TData> GetDecoder()
        {
            throw new NotImplementedException();
        }
    }
}
