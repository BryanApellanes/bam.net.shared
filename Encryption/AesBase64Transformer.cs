using Bam.Net.ServiceProxy.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesBase64Transformer : ValueTransformer<string, string>
    {
        public AesBase64Transformer(Func<AesKeyVectorPair> keyProvider)
        {
            this.KeyProvider = keyProvider;
            this.AesBase64ReverseTransformer = new AesBase64ReverseTransformer(this);
        }

        public AesBase64Transformer(IAesKeySource aesKeySource) : this(() => aesKeySource.GetAesKey())
        {
        }

        public AesBase64Transformer(AesKeyVectorPair aesKeyVectorPair) : this(() => aesKeyVectorPair)
        {
        }

        public AesBase64ReverseTransformer AesBase64ReverseTransformer { get; }

        public Func<AesKeyVectorPair> KeyProvider { get; set; }

        public override string ReverseTransform(string base64Cipher)
        {
            return GetReverseTransformer().ReverseTransform(base64Cipher);
        }

        public override string Transform(string plainText)
        {
            AesKeyVectorPair aesKey = KeyProvider();
            return aesKey.Encrypt(plainText);
        }

        public override IValueReverseTransformer<string, string> GetReverseTransformer()
        {
            return this.AesBase64ReverseTransformer;
        }
    }
}
