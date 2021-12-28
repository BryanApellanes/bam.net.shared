using Bam.Net;
using Bam.Net.Server.ServiceProxy.Data;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesUntransformer : IValueUntransformer<byte[], string>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public AesUntransformer()
        {
            this.AesEncoder = new AesTransformer() { AesDecoder = this };
        }

        public AesTransformer AesEncoder { get; internal set; }

        [Inject]
        public ISecureChannelSessionManager SecureChannelSessionManager { get; set; }

        public IHttpContext HttpContext { get; set; }
        public object Clone()
        {
            object clone = new AesUntransformer() { AesEncoder = AesEncoder };
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

        public string Untransform(byte[] cipherBytes)
        {
            SecureChannelSession session = SecureChannelSessionManager.GetSecureChannelSessionForContext(HttpContext);

            ClientSessionInfo clientSessionInfo = session.ToClientSessionInfo();
            return clientSessionInfo.GetPlainText(cipherBytes);
        }

        public IValueTransformer<string, byte[]> GetTransformer()
        {
            return this.AesEncoder;
        }
    }
}
