using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesBase64Transformer : ValueTransformer<string, string>
    {
        public Encoding Encoding { get; set; }
        public ClientSession ClientSessionInfo { get; set; }

        public override string Untransform(string base64Cipher)
        {
            return GetUntransformer().ReverseTransform(base64Cipher);
        }

        public override string Transform(string plainText)
        {
            return ClientSessionInfo.GetSymetricCipher(plainText);
        }

        public override IValueReverseTransformer<string, string> GetUntransformer()
        {
            throw new NotImplementedException();
        }
    }
}
