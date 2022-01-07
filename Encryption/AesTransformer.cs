using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesTransformer : ValueTransformer<string, byte[]>
    {
        public AesTransformer()
        {
            this.AesDecoder = new AesReverseTransformer() { AesEncoder = this } ;
        }

        public AesReverseTransformer AesDecoder { get; internal set; }
        public ClientSession ClientSessionInfo { get; set; }
        public override string Untransform(byte[] cipherBytes)
        {
            return GetUntransformer().ReverseTransform(cipherBytes);
        }

        public override byte[] Transform(string plainText)
        {
            return ClientSessionInfo.GetSymetricCipherBytes(plainText);
        }

        public override IValueReverseTransformer<byte[], string> GetUntransformer()
        {
            return this.AesDecoder;
        }
    }
}
