using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class ByteTransformer : ValueTransformer<byte[], byte[]>
    {
        public ByteTransformer()
        {
            this.Transformer = (b) => new byte[] { }; // noop
            this.ByteDecoder = new ByteReverseTransformer() { ByteTransformer = this };
        }

        public ByteTransformer(Func<byte[], byte[]> encoder):this()
        {
            this.Transformer = encoder;
        }

        public ByteReverseTransformer ByteDecoder { get; internal set; }

        public Func<byte[], byte[]> Transformer { get; set; }

        public override byte[] Untransform(byte[] output)
        {
            return GetUntransformer().ReverseTransform(output);
        }

        public override byte[] Transform(byte[] input)
        {
            return Transformer(input);
        }

        public override IValueReverseTransformer<byte[], byte[]> GetUntransformer()
        {
            return this.ByteDecoder;
        }
    }
}
