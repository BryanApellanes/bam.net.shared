using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaByteEncoder : ValueEncoder<byte[], byte[]>
    {
        public RsaByteEncoder()
        {
            this.RsaByteDecoder = new RsaByteDecoder() { RsaByteEncoder = this };
        }

        public ClientSessionInfo ClientSessionInfo { get; set; }

        public RsaByteDecoder RsaByteDecoder { get; set; }

        public override byte[] Decode(byte[] cipherBytes)
        {
            return GetDecoder().Decode(cipherBytes);
        }

        public override byte[] Encode(byte[] plainData)
        {
            Args.ThrowIfNull(ClientSessionInfo, $"{nameof(ClientSessionInfo)} not set");

            return ClientSessionInfo.GetAsymetricCipherBytes(plainData);
        }

        public override IValueDecoder<byte[], byte[]> GetDecoder()
        {
            return this.RsaByteDecoder;
        }
    }
}
