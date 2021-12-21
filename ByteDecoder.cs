using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class ByteDecoder : IValueDecoder<byte[], byte[]>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public ByteDecoder()
        {
            this.Decoder = (b) => new byte[] { }; // noop
            this.ByteEncoder = new ByteEncoder() { ByteDecoder = this };
        }

        public ByteDecoder(Func<byte[], byte[]> decoder) : this()
        {
            this.Decoder = decoder;
        }

        public Encoding Encoding { get; set; }

        public IHttpContext HttpContext { get; set; }

        public ByteEncoder ByteEncoder { get; internal set; }

        public object Clone()
        {
            object clone = new ByteDecoder() { ByteEncoder = this.ByteEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            ByteDecoder clone = new ByteDecoder() { ByteEncoder = this.ByteEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public Func<byte[], byte[]> Decoder { get; set; }

        public byte[] Decode(byte[] encoded)
        {
            return Decoder(encoded);
        }

        public IValueEncoder<byte[], byte[]> GetEncoder()
        {
            return this.ByteEncoder;
        }
    }
}
