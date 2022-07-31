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
            this.ByteReverseTransformer = new ByteReverseTransformer() { ByteTransformer = this };
        }

        public ByteTransformer(Func<byte[], byte[]> transformer):this()
        {
            this.Transformer = transformer;
        }

        public ByteReverseTransformer ByteReverseTransformer { get; internal set; }

        public Func<byte[], byte[]> Transformer { get; set; }

        public override byte[] ReverseTransform(byte[] output)
        {
            return GetReverseTransformer().ReverseTransform(output);
        }

        public override byte[] Transform(byte[] input)
        {
            return Transformer(input);
        }

        public override IValueReverseTransformer<byte[], byte[]> GetReverseTransformer()
        {
            return this.ByteReverseTransformer;
        }
    }
}
