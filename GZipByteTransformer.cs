using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    [PipelineFactoryTransformerName("gzip")]
    public class GZipByteTransformer : ValueTransformer<byte[], byte[]>
    {
        public GZipByteTransformer()
        {
            this.GZipByteReverseTransformer = new GZipByteReverseTransformer(this);
        }

        public GZipByteReverseTransformer GZipByteReverseTransformer { get; }

        public override byte[] ReverseTransform(byte[] tranformed)
        {
            return GetReverseTransformer().ReverseTransform(tranformed);
        }

        public override byte[] Transform(byte[] untransformed)
        {
            return untransformed.GZip();
        }

        public override IValueReverseTransformer<byte[], byte[]> GetReverseTransformer()
        {
            return this.GZipByteReverseTransformer;
        }
    }
}
