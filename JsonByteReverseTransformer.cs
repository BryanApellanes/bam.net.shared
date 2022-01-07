using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class JsonByteReverseTransformer<TData> : IValueReverseTransformer<byte[], TData>
    {
        public JsonByteReverseTransformer()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public TData ReverseTransform(byte[] encoded)
        {
            string json = Encoding.GetString(encoded);
            return json.FromJson<TData>();
        }

        public IValueTransformer<TData, byte[]> GetTransformer()
        {
            return new JsonByteTransformer<TData>();
        }
    }
}
