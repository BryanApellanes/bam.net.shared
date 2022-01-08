using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Data;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesByteReverseTransformer : IValueReverseTransformer<byte[], byte[]>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public AesByteReverseTransformer(AesByteTransformer aesByteTransformer)
        {
            this.Encoding = Encoding.UTF8;
            this.AesByteTransformer = aesByteTransformer;
        }

        public AesByteReverseTransformer(AesKeyVectorPair aesKeyVectorPair)
        {
            this.Encoding = Encoding.UTF8;
            this.KeyProvider = () => aesKeyVectorPair;
        }

        Func<AesKeyVectorPair> _keyProvider;
        public Func<AesKeyVectorPair> KeyProvider 
        {
            get
            {
                if (_keyProvider == null)
                {
                    if (this.AesByteTransformer != null && this.AesByteTransformer.KeyProvider != null)
                    {
                        this._keyProvider = this.AesByteTransformer.KeyProvider;
                    }
                }
                return _keyProvider;
            }
            set
            {
                _keyProvider = value;
            }
        }

        public Encoding Encoding { get; set; }

        public IHttpContext HttpContext { get; set; }

        public AesByteTransformer AesByteTransformer 
        {
            get;
            internal set;
        }

        public object Clone()
        {
            object clone = new AesByteReverseTransformer(AesByteTransformer);
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

        public byte[] ReverseTransform(byte[] cipherBytes)
        {
            Args.ThrowIfNull(KeyProvider, nameof(KeyProvider));
            AesKeyVectorPair aesKeyVectorPair = KeyProvider();

            return Aes.DecryptBytes(cipherBytes, aesKeyVectorPair.Key, aesKeyVectorPair.IV);
        }

        public IValueTransformer<byte[], byte[]> GetTransformer()
        {
            return this.AesByteTransformer;
        }
    }
}
