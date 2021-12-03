using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class JsonEncoder : Encoder
    {
        public JsonEncoder(Encoding encoding = null) : base(encoding)
        { 
        }

        public override T Decode<T>(byte[] bytes)
        {
            string json = Encoding.UTF8.GetString(bytes);
            return json.FromJson<T>();
        }

        public override byte[] Encode(object value)
        {
            string json = value.ToJson();
            return Encoding.GetBytes(json);
        }
    }
}
