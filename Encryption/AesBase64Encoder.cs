using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesBase64Encoder : ValueEncoder<string, string>
    {
        public Encoding Encoding { get; set; }
        public ClientSessionInfo ClientSessionInfo { get; set; }

        public override string Decode(string base64Cipher)
        {
            return GetDecoder().Decode(base64Cipher);
        }

        public override string Encode(string plainText)
        {
            return ClientSessionInfo.GetSymetricCipher(plainText);
        }

        public override IValueDecoder<string, string> GetDecoder()
        {
            throw new NotImplementedException();
        }
    }
}
