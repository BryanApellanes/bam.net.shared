using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipEncoder : ValueEncoder<string, byte[]>
    {
        public GZipEncoder()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public override string Decode(byte[] output)
        {
            return GetDecoder().Decode(output);
        }

        public override byte[] Encode(string input)
        {
            return input.GZip(Encoding);
        }

        public override IValueDecoder<byte[], string> GetDecoder()
        {
            return new GZipDecoder();
        }
    }
}
