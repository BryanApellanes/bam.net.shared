using Bam.Net.Server.ServiceProxy;
using Bam.Net.ServiceProxy.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaBase64Transformer : ValueTransformer<string, string>
    {
        public RsaBase64Transformer(Func<RsaPublicPrivateKeyPair> keyProvider)
        {
            this.Encoding = Encoding.UTF8;
            this.KeyProvider = keyProvider;
            this.RsaBase64ReverseTransformer = new RsaBase64ReverseTransformer(this);
        }

        public RsaBase64Transformer(IRsaKeySource rsaKeySource) : this(() => rsaKeySource.GetRsaKey())
        {
        }

        public RsaBase64Transformer(RsaPublicPrivateKeyPair rsaPublicPrivateKeyPair) : this(() => rsaPublicPrivateKeyPair)
        { 
        }

        protected RsaBase64ReverseTransformer RsaBase64ReverseTransformer { get; set; }

        public Encoding Encoding { get; set; }

        public Func<RsaPublicPrivateKeyPair> KeyProvider { get; set; }

        /// <summary>
        /// Converts the specified base 64 encoded cipher to plain text.
        /// </summary>
        /// <param name="base64Cipher"></param>
        /// <returns></returns>
        public override string ReverseTransform(string base64Cipher)
        {
            return GetReverseTransformer().ReverseTransform(base64Cipher);
        }

        /// <summary>
        /// Gets a base64 encoded cipher for the specified plain text.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public override string Transform(string plainText)
        {
            RsaPublicPrivateKeyPair rsaKey = KeyProvider();
            return rsaKey.Encrypt(plainText, Encoding);
        }

        public override IValueReverseTransformer<string, string> GetReverseTransformer()
        {
            return this.RsaBase64ReverseTransformer;
        }
    }
}
