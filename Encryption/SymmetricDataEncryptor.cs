using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class SymmetricDataEncryptor<TData> : ValueTransformerPipeline<TData>, IEncryptor<TData>
    {
        public SymmetricDataEncryptor(IAesKeySource aesKeySource)
        {
            this.AesByteTransformer = new AesByteTransformer(aesKeySource);

            this.Add(this.AesByteTransformer);
        }

        protected internal AesByteTransformer AesByteTransformer { get; private set; }

        public new SymmetricDataDecryptor<TData> GetReverseTransformer()
        {
            return new SymmetricDataDecryptor<TData>(this);
        }

        /// <summary>
        /// Encrypts the json representation of the specified data.
        /// </summary>
        /// <param name="data">The object data to encrypt.</param>
        /// <returns>byte[]</returns>
        public virtual Cipher<TData> Encrypt(TData data)
        {
            Cipher<TData> cipher = new Cipher<TData>
            {
                Data = Transform(data)
            };
            return cipher;
        }

        public IDecryptor<TData> GetDecryptor()
        {
            return GetReverseTransformer();
        }

        public string EncryptString(string plainData)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(plainData);
            byte[] cipherData = AesByteTransformer.Transform(utf8);

            return cipherData.ToBase64();
        }

        public byte[] EncryptBytes(byte[] plainData)
        {
            return AesByteTransformer.Transform(plainData);
        }
    }
}
