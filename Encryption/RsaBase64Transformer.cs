using Bam.Net.Server.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaBase64Transformer : ValueTransformer<string, string>
    {
        public RsaBase64Transformer()
        {
            Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }
        public ClientSession ClientSessionInfo { get; set; }

        /// <summary>
        /// Converts the specified base 64 encoded cipher to plain text.
        /// </summary>
        /// <param name="base64Cipher"></param>
        /// <returns></returns>
        public override string Untransform(string base64Cipher)
        {
            return GetUntransformer().ReverseTransform(base64Cipher);
        }

        /// <summary>
        /// Gets a base64 encoded cipher for the specified plain text.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public override string Transform(string plainText)
        {
            Args.ThrowIfNull(ClientSessionInfo, $"{nameof(ClientSessionInfo)} not set");

            return ClientSessionInfo.GetAsymetricCipher(plainText, Encoding);
        }

        public override IValueReverseTransformer<string, string> GetUntransformer()
        {
            return new RsaBase64ReverseTransformer();
        }
    }
}
