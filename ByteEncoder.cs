using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class ByteEncoder : ValueEncoder<byte[], byte[]>
    {
        public ByteEncoder()
        {
            this.Encoder = (b) => new byte[] { }; // noop
            this.ByteDecoder = new ByteDecoder() { ByteEncoder = this };
        }

        public ByteEncoder(Func<byte[], byte[]> encoder):this()
        {
            this.Encoder = encoder;
        }

        public ByteDecoder ByteDecoder { get; internal set; }

        public Func<byte[], byte[]> Encoder { get; set; }

        public override byte[] Decode(byte[] output)
        {
            return GetDecoder().Decode(output);
        }

        public override byte[] Encode(byte[] input)
        {
            return Encoder(input);
        }

        public override IValueDecoder<byte[], byte[]> GetDecoder()
        {
            return this.ByteDecoder;
        }
    }
}
