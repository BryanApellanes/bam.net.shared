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

        protected internal RsaByteTransformer RsaByteTransformer { get; private set; }
        protected GZipByteTransformer GZipByteTransformer { get; private set; }

        public new AsymmetricDecryptor<TData> GetReverseTransformer()
        {
            return new AsymmetricDecryptor<TData>(this);
        }

        /// <summary>
        /// Encrypts and gzips the json representation of the specified data.
        /// </summary>
        /// <param name="data">The object data to encrypt.</param>
        /// <returns>byte[]</returns>
        public Cipher<TData> Encrypt(TData data)
        {
            return Transform(data);
        }

        public IDecryptor<TData> GetDecryptor()
        {
            return GetReverseTransformer();
        }

        /// <summary>
        /// Encrypts the specified string and returns the base 64 encoded cipher.
        /// </summary>
        /// <param name="plainData"></param>
        /// <returns></returns>
        public string EncryptString(string plainData)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(plainData);
            byte[] cipherData = RsaByteTransformer.Transform(utf8);

            return cipherData.ToBase64();
        }

        public byte[] EncryptBytes(byte[] plainData)
        {
            return RsaByteTransformer.Transform(plainData);
        }
    }
}
