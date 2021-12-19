using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaEncoder : ValueEncoder<string, byte[]>
    {
        public RsaEncoder()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }
        public ClientSessionInfo ClientSessionInfo { get; set; }

        public override string Decode(byte[] cipherBytes)
        {
            return GetDecoder().Decode(cipherBytes);
        }

        public override byte[] Encode(string plainText)
        {
            Args.ThrowIfNull(ClientSessionInfo, $"{nameof(ClientSessionInfo)} not set");

            return ClientSessionInfo.GetAsymetricCipherBytes(plainText, Encoding);
        }

        public override IValueDecoder<byte[], string> GetDecoder()
        {
            return new RsaDecoder();
        }
    }
}
