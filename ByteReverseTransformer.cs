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
            this.Untransformer = (b) => new byte[] { }; // noop
            this.ByteTransformer = new ByteTransformer() { ByteDecoder = this };
        }

        public ByteReverseTransformer(Func<byte[], byte[]> decoder) : this()
        {
            this.Untransformer = decoder;
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

        public Func<byte[], byte[]> Untransformer { get; set; }

        public byte[] ReverseTransform(byte[] transformed)
        {
            return Untransformer(transformed);
        }

        public IValueTransformer<byte[], byte[]> GetTransformer()
        {
            return this.ByteTransformer;
        }
    }
}
