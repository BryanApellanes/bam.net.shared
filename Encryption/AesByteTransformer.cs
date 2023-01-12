using Bam.Net.ServiceProxy.Data.Dao.Repository;
using Bam.Net.ServiceProxy.Encryption;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    [PipelineFactoryTransformerName("aes")]
    public class AesByteTransformer : ValueTransformer<byte[], byte[]>
    {
        public AesByteTransformer(Func<AesKeyVectorPair> keyProvider)
        {
            this.AesByteReverseTransformer = new AesByteReverseTransformer(this);
            this.KeyProvider = keyProvider;
        }

        [PipelineFactoryConstructor]
        public AesByteTransformer(IAesKeySource aesKeySource) : this(() => aesKeySource.GetAesKey())
        { 
        }

        public AesByteTransformer(AesKeyVectorPair aesKeyVectorPair) : this(() => aesKeyVectorPair)
        { 
        }

        AesByteReverseTransformer _aesByteUntransformer;
        protected AesByteReverseTransformer AesByteReverseTransformer 
        {
            get
            {
                if (this._aesByteUntransformer == null)
                {
                    this._aesByteUntransformer = new AesByteReverseTransformer(this);
                }

                return this._aesByteUntransformer;
            }

            set
            {
                this._aesByteUntransformer = value;
            }
        }

        public Func<AesKeyVectorPair> KeyProvider { get; set; }

        public override byte[] ReverseTransform(byte[] cipherBytes)
        {
            return GetReverseTransformer().ReverseTransform(cipherBytes);
        }

        public override byte[] Transform(byte[] plainData)
        {
            Args.ThrowIfNull(KeyProvider, nameof(KeyProvider));
            AesKeyVectorPair aesKey = KeyProvider();

            return aesKey.EncryptBytes(plainData);
        }

        public override IValueReverseTransformer<byte[], byte[]> GetReverseTransformer()
        {
            return this.AesByteReverseTransformer;
        }
    }
}
