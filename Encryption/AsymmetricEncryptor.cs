using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AsymmetricEncryptor<TData> : ValueTransformerPipeline<TData>, IEncryptor<TData>
    {
        public AsymmetricEncryptor(IRsaPublicKeySource rsaPublicKeySource)
        {
            this.RsaByteTransformer = new RsaByteTransformer(rsaPublicKeySource);
            this.GZipByteTransformer = new GZipByteTransformer();

            this.Add(this.RsaByteTransformer);
            this.Add(this.GZipByteTransformer);
        }

        public AsymmetricEncryptor(RsaPublicPrivateKeyPair rsaPublicPrivateKeyPair)
        {
            this.RsaByteTransformer = new RsaByteTransformer(rsaPublicPrivateKeyPair);
            this.GZipByteTransformer = new GZipByteTransformer();

            this.Add(this.RsaByteTransformer);
            this.Add(this.GZipByteTransformer);
        }

        protected RsaByteTransformer RsaByteTransformer { get; private set; }
        protected GZipByteTransformer GZipByteTransformer { get; private set; }

        public new AsymmetricDecryptor<TData> GetReverseTransformer()
        {
            return new AsymmetricDecryptor<TData>(this);
        }

        public byte[] Encrypt(TData data)
        {
            return Transform(data);
        }

        public IDecryptor<TData> GetDecryptor()
        {
            return GetReverseTransformer();
        }
    }
}
