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
            this.RsaByteDecoder = new RsaByteUntransformer() { RsaByteEncoder = this };
        }

        public ClientSessionInfo ClientSessionInfo { get; set; }

        public RsaByteUntransformer RsaByteDecoder { get; set; }

        public override byte[] Untransform(byte[] cipherBytes)
        {
            return GetUntransformer().Untransform(cipherBytes);
        }

        public override byte[] Transform(byte[] plainData)
        {
            Args.ThrowIfNull(ClientSessionInfo, $"{nameof(ClientSessionInfo)} not set");

            return ClientSessionInfo.GetAsymetricCipherBytes(plainData);
        }

        public override IValueUntransformer<byte[], byte[]> GetUntransformer()
        {
            return this.RsaByteDecoder;
        }
    }
}
