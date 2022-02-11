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
    public class RsaByteReverseTransformer : IValueReverseTransformer<byte[], byte[]>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public RsaByteReverseTransformer(Func<RsaPublicPrivateKeyPair> keyProvider)
        {
            this.KeyProvider = keyProvider;
        }
        public RsaByteReverseTransformer(RsaByteTransformer rsaByteReverseTransformer)
        {
            this.RsaByteEncoder = rsaByteReverseTransformer;
            this.KeyProvider = rsaByteReverseTransformer.KeyProvider;
        }

        public Func<RsaPublicPrivateKeyPair> KeyProvider { get; set; }

        public RsaByteTransformer RsaByteEncoder { get; set; }

        public IHttpContext HttpContext { get; set; }

        public object Clone()
        {
            object clone = new RsaByteReverseTransformer(RsaByteEncoder);
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            RsaByteReverseTransformer clone = new RsaByteReverseTransformer(RsaByteEncoder);
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public byte[] ReverseTransform(byte[] cipherBytes)
        {
            RsaPublicPrivateKeyPair rsaKey = KeyProvider();

            return rsaKey.DecryptBytes(cipherBytes);
        }

        public IValueTransformer<byte[], byte[]> GetTransformer()
        {
            return this.RsaByteEncoder;
        }
    }
}
