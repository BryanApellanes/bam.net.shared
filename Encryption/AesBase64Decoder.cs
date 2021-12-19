using Bam.Net.Server.ServiceProxy.Data;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesBase64Decoder : IValueDecoder<string, string>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        [Inject]
        public ISecureChannelSessionManager SecureChannelSessionManager { get; set; }

        public Encoding Encoding { get; set; }
        public IHttpContext HttpContext { get; set; }

        public object Clone()
        {
            object clone = new RsaBase64Decoder();
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            RsaBase64Decoder clone = new RsaBase64Decoder();
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public string Decode(string base64EncodedCipher)
        {
            byte[] cipherBytes = base64EncodedCipher.FromBase64();
            SecureChannelSession session = SecureChannelSessionManager.GetSecureChannelSessionForContext(HttpContext);

            ClientSessionInfo clientSessionInfo = session.ToClientSessionInfo();
            return clientSessionInfo.GetPlainText(cipherBytes);
        }

        public IValueEncoder<string, string> GetEncoder()
        {
            return new AesBase64Encoder();
        }
    }
}
