using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class JsonByteUntransformer<TData> : IValueUntransformer<byte[], TData>
    {
        public JsonByteUntransformer()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public TData Untransform(byte[] encoded)
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
