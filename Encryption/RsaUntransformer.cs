using Bam.Net.ServiceProxy.Data.Dao.Repository;
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
    public class RsaUntransformer : IValueUntransformer<byte[], string>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public RsaUntransformer()
        {
            this.Encoding = Encoding.UTF8;
        }

        [Inject]
        public ISecureChannelSessionDataManager SecureChannelSessionManager { get; set; }

        public Encoding Encoding { get; set; }
        public IHttpContext HttpContext { get; set; }

        public object Clone()
        {
            object clone = new RsaUntransformer();
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            RsaUntransformer clone = new RsaUntransformer();
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public virtual string Untransform(byte[] cipherBytes)
        {
            SecureChannelSession session = SecureChannelSessionManager.GetSecureChannelSessionForContextAsync(HttpContext).Result;

            AsymmetricCipherKeyPair keyPair = session.AsymmetricKey.ToKeyPair();
            byte[] decryptedBytes = cipherBytes.DecryptWithPrivateKey(keyPair.Private);
            return Encoding.GetString(decryptedBytes);
        }

        public virtual IValueTransformer<string, byte[]> GetTransformer()
        {
            return new RsaTransformer();
        }
    }
}
