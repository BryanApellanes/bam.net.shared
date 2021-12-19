using Bam.Net.Server.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaBase64Encoder : ValueEncoder<string, string>
    {
        public RsaBase64Encoder()
        {
            Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }
        public ClientSessionInfo ClientSessionInfo { get; set; }

        /// <summary>
        /// Converts the specified base 64 encoded cipher to plain text.
        /// </summary>
        /// <param name="base64Cipher"></param>
        /// <returns></returns>
        public override string Decode(string base64Cipher)
        {
            return GetDecoder().Decode(base64Cipher);
        }

        /// <summary>
        /// Gets a base64 encoded cipher for the specified plain text.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public override string Encode(string plainText)
        {
            Args.ThrowIfNull(ClientSessionInfo, $"{nameof(ClientSessionInfo)} not set");

            return ClientSessionInfo.GetAsymetricCipher(plainText, Encoding);
        }

        public override IValueDecoder<string, string> GetDecoder()
        {
            return new RsaBase64Decoder();
        }
    }
}
