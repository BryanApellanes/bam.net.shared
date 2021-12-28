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

        public override string Untransform(byte[] output)
        {
            return GetUntransformer().Untransform(output);
        }

        public override byte[] Transform(string input)
        {
            return input.GZip(Encoding);
        }

        public override IValueUntransformer<byte[], string> GetUntransformer()
        {
            return new GZipUntransformer();
        }
    }
}
