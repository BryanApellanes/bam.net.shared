using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class JsonDecoder<TDecoded> : IValueDecoder<string, TDecoded>
    {
        public TDecoded Decode(string encoded)
        {
            return encoded.FromJson<TDecoded>();
        }

        public IValueEncoder<TDecoded, string> GetEncoder()
        {
            return (IValueEncoder<TDecoded, string>)new JsonEncoder<string>();
        }
    }
}
