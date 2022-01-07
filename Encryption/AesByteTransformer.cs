using Bam.Net.ServiceProxy.Data.Dao.Repository;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesByteTransformer : ValueTransformer<byte[], byte[]>
    {
        public AesByteTransformer(Func<AesKeyVectorPair> keyProvider)
        {
            this.AesByteUntransformer = new AesByteReverseTransformer(this);
            this.KeyProvider = keyProvider;
        }

        public AesByteTransformer(IAesKeySource aesKeySource) : this(() => aesKeySource.GetAesKey())
        { 
        }

        AesByteReverseTransformer _aesByteUntransformer;
        public AesByteReverseTransformer AesByteUntransformer 
        {
            get
            {
                if (this._aesByteUntransformer == null)
                {
                    this._aesByteUntransformer = new AesByteReverseTransformer(this);
                }

                return this._aesByteUntransformer;
            }

            internal set
            {
                this._aesByteUntransformer = value;
            }
        }

        public Func<AesKeyVectorPair> KeyProvider { get; set; }

        public override byte[] Untransform(byte[] cipherBytes)
        {
            return GetUntransformer().ReverseTransform(cipherBytes);
        }

        public override byte[] Transform(byte[] plainData)
        {
            Args.ThrowIfNull(KeyProvider, nameof(KeyProvider));
            AesKeyVectorPair aesKey = KeyProvider();

            return Aes.EncryptBytes(plainData, aesKey.Key, aesKey.IV);
        }

        public override IValueReverseTransformer<byte[], byte[]> GetUntransformer()
        {
            return this.AesByteUntransformer;
        }
    }
}
