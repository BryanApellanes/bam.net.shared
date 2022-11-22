using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class ByteReverseTransformer : IValueReverseTransformer<byte[], byte[]>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public ByteReverseTransformer()
        {
            this.ReverseTransformer = (b) => new byte[] { }; // noop
            this.ByteTransformer = new ByteTransformer() { ByteReverseTransformer = this };
        }

        public ByteReverseTransformer(Func<byte[], byte[]> decoder) : this()
        {
            this.ReverseTransformer = decoder;
        }

        public Encoding Encoding { get; set; }

        public IHttpContext HttpContext { get; set; }

        public ByteTransformer ByteTransformer { get; internal set; }

        public object Clone()
        {
            object clone = new ByteReverseTransformer() { ByteTransformer = this.ByteTransformer };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            ByteReverseTransformer clone = new ByteReverseTransformer() { ByteTransformer = this.ByteTransformer };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public Func<byte[], byte[]> ReverseTransformer { get; set; }

        public byte[] ReverseTransform(byte[] transformed)
        {
            return ReverseTransformer(transformed);
        }

        public IValueTransformer<byte[], byte[]> GetTransformer()
        {
            return this.ByteTransformer;
        }
    }
}
