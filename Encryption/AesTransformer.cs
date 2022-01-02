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
            this.AesDecoder = new AesUntransformer() { AesEncoder = this } ;
        }

        public AesUntransformer AesDecoder { get; internal set; }
        public ClientSession ClientSessionInfo { get; set; }
        public override string Untransform(byte[] cipherBytes)
        {
            return GetUntransformer().Untransform(cipherBytes);
        }

        public override byte[] Transform(string plainText)
        {
            return ClientSessionInfo.GetSymetricCipherBytes(plainText);
        }

        public override IValueUntransformer<byte[], string> GetUntransformer()
        {
            return this.AesDecoder;
        }
    }
}
