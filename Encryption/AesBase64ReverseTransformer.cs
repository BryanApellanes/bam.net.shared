using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Data;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesBase64ReverseTransformer : IValueReverseTransformer<string, string>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        [Inject]
        public ISecureChannelSessionDataManager SecureChannelSessionDataManager { get; set; }

        public Encoding Encoding { get; set; }
        public IHttpContext HttpContext { get; set; }

        public object Clone()
        {
            object clone = new RsaBase64ReverseTransformer();
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            RsaBase64ReverseTransformer clone = new RsaBase64ReverseTransformer();
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
            byte[] cipherBytes = base64EncodedCipher.FromBase64();
            SecureChannelSession session = SecureChannelSessionDataManager.GetSecureChannelSessionForContextAsync(HttpContext).Result;

            ClientSession clientSessionInfo = session.GetClientSession();
            return clientSessionInfo.GetPlainText(cipherBytes);
        }

        public IValueTransformer<string, string> GetTransformer()
        {
            return new AesBase64Transformer();
        }
    }
}
