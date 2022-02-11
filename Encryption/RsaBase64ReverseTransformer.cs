using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Data;
using Bam.Net.ServiceProxy.Encryption;
using Bam.Net.Services;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaBase64ReverseTransformer : IValueReverseTransformer<string, string>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public RsaBase64ReverseTransformer(RsaBase64Transformer transformer)
        {
            this.RsaBase64Transformer = transformer;
            this.KeyProvider = transformer.KeyProvider;
        }

        protected RsaBase64Transformer RsaBase64Transformer { get; set; }
        public Func<RsaPublicPrivateKeyPair> KeyProvider { get; set; }
        public Encoding Encoding { get; set; }
        public IHttpContext HttpContext { get; set; }

        public object Clone()
        {
            object clone = new RsaBase64ReverseTransformer(RsaBase64Transformer);
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            RsaBase64ReverseTransformer clone = new RsaBase64ReverseTransformer(RsaBase64Transformer);
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public string ReverseTransform(string base64Cipher)
        {
            RsaPublicPrivateKeyPair rsaKey = KeyProvider();
            return rsaKey.Decrypt(base64Cipher, Encoding);
        }

        public IValueTransformer<string, string> GetTransformer()
        {
            return this.RsaBase64Transformer;
        }
    }
}
