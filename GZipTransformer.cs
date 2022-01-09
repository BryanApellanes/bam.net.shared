using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipTransformer : ValueTransformer<string, byte[]>
    {
        public GZipTransformer()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public override string ReverseTransform(byte[] output)
        {
            return GetReverseTransformer().ReverseTransform(output);
        }

        public override byte[] Transform(string input)
        {
            return input.GZip(Encoding);
        }

        public override IValueReverseTransformer<byte[], string> GetReverseTransformer()
        {
            return new GZipReverseTransformer();
        }
    }
}
