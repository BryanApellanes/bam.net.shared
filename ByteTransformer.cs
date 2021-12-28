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
            this.ByteDecoder = new ByteUntransformer() { ByteTransformer = this };
        }

        public ByteTransformer(Func<byte[], byte[]> encoder):this()
        {
            this.Transformer = encoder;
        }

        public ByteUntransformer ByteDecoder { get; internal set; }

        public Func<byte[], byte[]> Transformer { get; set; }

        public override byte[] Untransform(byte[] output)
        {
            return GetUntransformer().Untransform(output);
        }

        public override byte[] Transform(byte[] input)
        {
            return Transformer(input);
        }

        public override IValueUntransformer<byte[], byte[]> GetUntransformer()
        {
            return this.ByteDecoder;
        }
    }
}
