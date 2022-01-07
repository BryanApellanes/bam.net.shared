using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Data;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Services;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaByteReverseTransformer : IValueReverseTransformer<byte[], byte[]>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public RsaByteReverseTransformer()
        {
            this.RsaByteEncoder = new RsaByteTransformer() { RsaByteDecoder = this };
        }

        [Inject]
        public ISecureChannelSessionDataManager SecureChannelSessionManager { get; set; }

        public RsaByteTransformer RsaByteEncoder { get; set; }

        public IHttpContext HttpContext { get; set; }

        public object Clone()
        {
            object clone = new RsaReverseTransformer();
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            RsaReverseTransformer clone = new RsaReverseTransformer();
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
            SecureChannelSession session = SecureChannelSessionManager.GetSecureChannelSessionForContextAsync(HttpContext).Result;

            AsymmetricCipherKeyPair keyPair = session.AsymmetricKey.ToKeyPair();
            byte[] decryptedBytes = cipherBytes.DecryptWithPrivateKey(keyPair.Private);
            return decryptedBytes;
        }

        public IValueTransformer<byte[], byte[]> GetTransformer()
        {
            return this.RsaByteEncoder;
        }
    }
}
