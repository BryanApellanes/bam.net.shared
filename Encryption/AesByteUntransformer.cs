using Bam.Net.Server.ServiceProxy.Data;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesByteUntransformer : IValueUntransformer<byte[], byte[]>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public AesByteUntransformer()
        {
            this.Encoding = Encoding.UTF8;
            this.AesByteEncoder = new AesByteTransformer() { AesByteDecoder = this };
        }

        public Encoding Encoding { get; set; }

        [Inject]
        public ISecureChannelSessionManager SecureChannelSessionManager { get; set; }

        public IHttpContext HttpContext { get; set; }

        public AesByteTransformer AesByteEncoder { get; internal set; }
        public object Clone()
        {
            object clone = new AesByteUntransformer() { AesByteEncoder = AesByteEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            AesUntransformer clone = new AesUntransformer();
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public byte[] Untransform(byte[] cipherBytes)
        {
            SecureChannelSession session = SecureChannelSessionManager.GetSecureChannelSessionForContext(HttpContext);

            ClientSessionInfo clientSessionInfo = session.ToClientSessionInfo();

            return clientSessionInfo.GetPlainBytes(cipherBytes, Encoding);
        }

        public IValueTransformer<byte[], byte[]> GetTransformer()
        {
            return this.AesByteEncoder;
        }
    }
}
