using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class JsonByteDecoder<TData> : IValueDecoder<byte[], TData>, ICloneable
    {
        public JsonByteDecoder()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }


        public JsonByteEncoder<TData> JsonByteEncoder { get; set; }

        public object Clone()
        {
            object clone = new JsonByteDecoder<TData>() { JsonByteEncoder = JsonByteEncoder};
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            JsonByteDecoder<TData> clone = new JsonByteDecoder<TData>() { JsonByteEncoder = JsonByteEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public TData Decode(byte[] encoded)
        {
            string json = Encoding.GetString(encoded);
            return json.FromJson<TData>();
        }

        public IValueEncoder<TData, byte[]> GetEncoder()
        {
            return this.JsonByteEncoder;
        }
    }
}
