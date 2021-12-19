using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class Base64Decoder : IValueDecoder<string, byte[]>
    {
        public byte[] Decode(string encoded)
        {
            return encoded.FromBase64();
        }

        public IValueEncoder<byte[], string> GetEncoder()
        {
            return (IValueEncoder<byte[], string>)new Base64Encoder();
        }
    }
}
