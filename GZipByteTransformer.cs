using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipByteTransformer : ValueTransformer<byte[], byte[]>
    {
        public GZipByteTransformer()
        {
            this.GZipByteReverseTransformer = new GZipByteReverseTransformer() { GZipByteTransformer = this };
        }

        public GZipByteReverseTransformer GZipByteReverseTransformer { get; set; }

        public override byte[] Untransform(byte[] output)
        {
            return GetUntransformer().ReverseTransform(output);
        }

        public override byte[] Transform(byte[] input)
        {
            return input.GZip();
        }

        public override IValueReverseTransformer<byte[], byte[]> GetUntransformer()
        {
            return this.GZipByteReverseTransformer;
        }
    }
}
