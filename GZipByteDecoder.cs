using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipByteDecoder : IValueDecoder<byte[], byte[]>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public GZipByteDecoder()
        {
            this.GZipByteEncoder = new GZipByteEncoder() { GZipByteDecoder = this };
        }

        public IHttpContext HttpContext { get; set; }

        public GZipByteEncoder GZipByteEncoder { get; set; }

        public object Clone()
        {
            object clone = new GZipByteDecoder() { GZipByteEncoder = GZipByteEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            GZipByteDecoder clone = new GZipByteDecoder() { GZipByteEncoder = GZipByteEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public byte[] Decode(byte[] encoded)
        {
            return encoded.GUnzip();
        }

        public IValueEncoder<byte[], byte[]> GetEncoder()
        {
            return this.GZipByteEncoder;
        }
    }
}
