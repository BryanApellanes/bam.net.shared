using Bam.Net.Server.ServiceProxy.Data;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Services;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaByteDecoder : IValueDecoder<byte[], byte[]>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public RsaByteDecoder()
        {
            this.RsaByteEncoder = new RsaByteEncoder() { RsaByteDecoder = this };
        }

        [Inject]
        public ISecureChannelSessionManager SecureChannelSessionManager { get; set; }

        public RsaByteEncoder RsaByteEncoder { get; set; }

        public IHttpContext HttpContext { get; set; }

        public object Clone()
        {
            object clone = new RsaDecoder();
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            RsaDecoder clone = new RsaDecoder();
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public byte[] Decode(byte[] cipherBytes)
        {
            SecureChannelSession session = SecureChannelSessionManager.GetSecureChannelSessionForContext(HttpContext);

            AsymmetricCipherKeyPair keyPair = session.AsymmetricKey.ToKeyPair();
            byte[] decryptedBytes = cipherBytes.DecryptWithPrivateKey(keyPair.Private);
            return decryptedBytes;
        }

        public IValueEncoder<byte[], byte[]> GetEncoder()
        {
            return this.RsaByteEncoder;
        }
    }
}
