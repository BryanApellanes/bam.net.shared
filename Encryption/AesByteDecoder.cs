using Bam.Net.Server.ServiceProxy.Data;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesByteDecoder : IValueDecoder<byte[], byte[]>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public AesByteDecoder()
        {
            this.Encoding = Encoding.UTF8;
            this.AesByteEncoder = new AesByteEncoder() { AesByteDecoder = this };
        }

        public Encoding Encoding { get; set; }

        [Inject]
        public ISecureChannelSessionManager SecureChannelSessionManager { get; set; }

        public IHttpContext HttpContext { get; set; }

        public AesByteEncoder AesByteEncoder { get; internal set; }
        public object Clone()
        {
            object clone = new AesByteDecoder() { AesByteEncoder = AesByteEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            AesDecoder clone = new AesDecoder();
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

            ClientSessionInfo clientSessionInfo = session.ToClientSessionInfo();

            return clientSessionInfo.GetPlainBytes(cipherBytes, Encoding);
        }

        public IValueEncoder<byte[], byte[]> GetEncoder()
        {
            return this.AesByteEncoder;
        }
    }
}
