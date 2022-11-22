using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipByteReverseTransformer : IValueReverseTransformer<byte[], byte[]>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public GZipByteReverseTransformer(GZipByteTransformer gZipByteTransformer)
        {
            this.GZipByteTransformer = gZipByteTransformer;
        }

        public IHttpContext HttpContext { get; set; }

        public GZipByteTransformer GZipByteTransformer { get; }

        public object Clone()
        {
            object clone = new GZipByteReverseTransformer(GZipByteTransformer);
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            GZipByteReverseTransformer clone = new GZipByteReverseTransformer(GZipByteTransformer);
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public byte[] ReverseTransform(byte[] transformed)
        {
            return transformed.GUnzip();
        }

        public IValueTransformer<byte[], byte[]> GetTransformer()
        {
            return this.GZipByteTransformer;
        }
    }
}
