using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipByteTransformer : ValueTransformer<byte[], byte[]>
    {
        public GZipByteTransformer()
        {
            this.GZipByteDecoder = new GZipByteUntransformer() { GZipByteEncoder = this };
        }

        public GZipByteUntransformer GZipByteDecoder { get; set; }

        public override byte[] Untransform(byte[] output)
        {
            return GetUntransformer().Untransform(output);
        }

        public override byte[] Transform(byte[] input)
        {
            return input.GZip();
        }

        public override IValueUntransformer<byte[], byte[]> GetUntransformer()
        {
            return this.GZipByteDecoder;
        }
    }
}
