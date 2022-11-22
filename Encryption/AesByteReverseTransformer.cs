using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Data;
using Bam.Net.ServiceProxy.Encryption;
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
            this.AesByteTransformer = aesByteTransformer;
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

        public IHttpContext HttpContext { get; set; }

        protected AesByteTransformer AesByteTransformer 
        {
            get;
            set;
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
            AesByteReverseTransformer clone = new AesByteReverseTransformer(AesByteTransformer);
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

            return aesKeyVectorPair.DecryptBytes(cipherBytes);
        }

        public IValueTransformer<byte[], byte[]> GetTransformer()
        {
            return this.AesByteTransformer;
        }
    }
}
