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
    public class RsaBase64ReverseTransformer : IValueReverseTransformer<string, string>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        [Inject]
        public ISecureChannelSessionDataManager SecureChannelSessionManager { get; set; }

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

        public string ReverseTransform(string base64Cipher)
        {
            byte[] cipherBytes = base64Cipher.FromBase64();

            SecureChannelSession session = SecureChannelSessionManager.GetSecureChannelSessionForContextAsync(HttpContext).Result;

            AsymmetricCipherKeyPair keyPair = session.AsymmetricKey.ToKeyPair();
            byte[] decryptedBytes = cipherBytes.DecryptWithPrivateKey(keyPair.Private);
            return Encoding.GetString(decryptedBytes);
        }

        public IValueTransformer<string, string> GetTransformer()
        {
            return new RsaBase64Transformer();
        }
    }
}
