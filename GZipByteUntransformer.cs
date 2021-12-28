using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipByteUntransformer : IValueUntransformer<byte[], byte[]>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public GZipByteUntransformer()
        {
            this.GZipByteEncoder = new GZipByteTransformer() { GZipByteDecoder = this };
        }

        public IHttpContext HttpContext { get; set; }

        public GZipByteTransformer GZipByteEncoder { get; set; }

        public object Clone()
        {
            object clone = new GZipByteUntransformer() { GZipByteEncoder = GZipByteEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            GZipByteUntransformer clone = new GZipByteUntransformer() { GZipByteEncoder = GZipByteEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public byte[] Untransform(byte[] encoded)
        {
            return encoded.GUnzip();
        }

        public IValueTransformer<byte[], byte[]> GetTransformer()
        {
            return this.GZipByteEncoder;
        }
    }
}
