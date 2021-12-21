using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesByteEncoder : ValueEncoder<byte[], byte[]>
    {
        public AesByteDecoder AesByteDecoder { get; internal set; }
        public ClientSessionInfo ClientSessionInfo { get; set; }

        public override byte[] Decode(byte[] cipherBytes)
        {
            return GetDecoder().Decode(cipherBytes);
        }

        public override byte[] Encode(byte[] plainData)
        {
            return Aes.EncryptBytes(plainData, ClientSessionInfo.SessionKey, ClientSessionInfo.SessionIV);
        }

        public override IValueDecoder<byte[], byte[]> GetDecoder()
        {
            return this.AesByteDecoder;
        }
    }
}
