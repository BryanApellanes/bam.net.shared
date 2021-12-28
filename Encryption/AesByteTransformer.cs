using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesByteTransformer : ValueTransformer<byte[], byte[]>
    {
        public AesByteUntransformer AesByteDecoder { get; internal set; }
        public ClientSessionInfo ClientSessionInfo { get; set; }

        public override byte[] Untransform(byte[] cipherBytes)
        {
            return GetUntransformer().Untransform(cipherBytes);
        }

        public override byte[] Transform(byte[] plainData)
        {
            return Aes.EncryptBytes(plainData, ClientSessionInfo.SessionKey, ClientSessionInfo.SessionIV);
        }

        public override IValueUntransformer<byte[], byte[]> GetUntransformer()
        {
            return this.AesByteDecoder;
        }
    }
}
