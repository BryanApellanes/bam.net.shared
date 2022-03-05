using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class SymmetricEncryptor<TData> : ValueTransformerPipeline<TData>, IEncryptor<TData>
    {
        public SymmetricEncryptor(IAesKeySource aesKeySource)
        {
            this.AesByteTransformer = new AesByteTransformer(aesKeySource);
            this.GZipByteTransformer = new GZipByteTransformer();

            this.Add(this.AesByteTransformer);
            this.Add(this.GZipByteTransformer);
        }

        protected AesByteTransformer AesByteTransformer { get; private set; }
        protected GZipByteTransformer GZipByteTransformer { get; private set; }

        public new SymmetricDecryptor<TData> GetReverseTransformer()
        {
            return new SymmetricDecryptor<TData>(this);
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
