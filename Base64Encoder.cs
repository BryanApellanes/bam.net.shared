using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class Base64Encoder : ValueEncoder<byte[], string>
    {
        public override byte[] Decode(string output)
        {
            return output.FromBase64();
        }

        public override string Encode(byte[] input)
        {
            return input.ToBase64();
        }

        public override IValueDecoder<string, byte[]> GetDecoder()
        {
            return new Base64Decoder();
        }
    }
}
