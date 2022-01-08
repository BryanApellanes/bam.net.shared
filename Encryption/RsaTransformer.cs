using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaTransformer : ValueTransformer<string, byte[]>
    {
        public RsaTransformer()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }
        public ClientSession ClientSessionInfo { get; set; }

        public override string Untransform(byte[] cipherBytes)
        {
            return GetReverseTransformer().ReverseTransform(cipherBytes);
        }

        public override byte[] Transform(string plainText)
        {
            Args.ThrowIfNull(ClientSessionInfo, $"{nameof(ClientSessionInfo)} not set");

            return ClientSessionInfo.GetAsymetricCipherBytes(plainText, Encoding);
        }

        public override IValueReverseTransformer<byte[], string> GetReverseTransformer()
        {
            return new RsaReverseTransformer();
        }
    }
}
