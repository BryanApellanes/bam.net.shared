using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipByteEncoder : ValueEncoder<byte[], byte[]>
    {
        public GZipByteEncoder()
        {
            this.GZipByteDecoder = new GZipByteDecoder() { GZipByteEncoder = this };
        }

        public GZipByteDecoder GZipByteDecoder { get; set; }

        public override byte[] Decode(byte[] output)
        {
            return GetDecoder().Decode(output);
        }

        public override byte[] Encode(byte[] input)
        {
            return input.GZip();
        }

        public override IValueDecoder<byte[], byte[]> GetDecoder()
        {
            return this.GZipByteDecoder;
        }
    }
}
