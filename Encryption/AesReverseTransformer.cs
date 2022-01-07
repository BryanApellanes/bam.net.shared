using Bam.Net;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Data;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesReverseTransformer : IValueReverseTransformer<byte[], string>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public AesReverseTransformer()
        {
            this.AesEncoder = new AesTransformer() { AesDecoder = this };
        }

        public AesTransformer AesEncoder { get; internal set; }

        [Inject]
        public ISecureChannelSessionDataManager SecureChannelSessionDataManager { get; set; }

        public IHttpContext HttpContext { get; set; }
        public object Clone()
        {
            object clone = new AesReverseTransformer() { AesEncoder = AesEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            AesReverseTransformer clone = new AesReverseTransformer();
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public string ReverseTransform(byte[] cipherBytes)
        {
            SecureChannelSession session = SecureChannelSessionDataManager.GetSecureChannelSessionForContextAsync(HttpContext).Result;

            ClientSession clientSessionInfo = session.GetClientSession();
            return clientSessionInfo.GetPlainText(cipherBytes);
        }

        public IValueTransformer<string, byte[]> GetTransformer()
        {
            return this.AesEncoder;
        }
    }
}
