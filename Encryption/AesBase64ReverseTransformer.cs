using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Data;
using Bam.Net.ServiceProxy.Encryption;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesBase64ReverseTransformer : IValueReverseTransformer<string, string>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public AesBase64ReverseTransformer(AesBase64Transformer aesBase64Transformer, Encoding encoding = null)
        {
            this.AesBase64Transformer = aesBase64Transformer;
            this.KeyProvider = aesBase64Transformer.KeyProvider;
            this.Encoding = encoding ?? Encoding.UTF8;
        }

        protected AesBase64Transformer AesBase64Transformer { get; set; }

        public Func<AesKeyVectorPair> KeyProvider { get; set; }

        public Encoding Encoding { get; set; }
        public IHttpContext HttpContext { get; set; }

        public object Clone()
        {
            object clone = new AesBase64ReverseTransformer(this.AesBase64Transformer, Encoding);
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            AesBase64ReverseTransformer clone = new AesBase64ReverseTransformer(this.AesBase64Transformer, Encoding);
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public string ReverseTransform(string base64EncodedCipher)
        {
            AesKeyVectorPair aesKeyVectorPair = KeyProvider();
            byte[] cipherBytes = Convert.FromBase64String(base64EncodedCipher);
            byte[] decipheredBytes = aesKeyVectorPair.DecryptBytes(cipherBytes);

            return Encoding.GetString(decipheredBytes);
        }

        public IValueTransformer<string, string> GetTransformer()
        {
            return this.AesBase64Transformer;
        }
    }
}
