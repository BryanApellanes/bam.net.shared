using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaByteTransformer : ValueTransformer<byte[], byte[]>
    {
        public RsaByteTransformer()
        {
            this.RsaByteDecoder = new RsaByteReverseTransformer() { RsaByteEncoder = this };
        }

        public ClientSession ClientSessionInfo { get; set; }

        public RsaByteReverseTransformer RsaByteDecoder { get; set; }

        public override byte[] Untransform(byte[] cipherBytes)
        {
            return GetReverseTransformer().ReverseTransform(cipherBytes);
        }

        public override byte[] Transform(byte[] plainData)
        {
            Args.ThrowIfNull(ClientSessionInfo, $"{nameof(ClientSessionInfo)} not set");

            return ClientSessionInfo.GetAsymetricCipherBytes(plainData);
        }

        public override IValueReverseTransformer<byte[], byte[]> GetReverseTransformer()
        {
            return this.RsaByteDecoder;
        }
    }
}
