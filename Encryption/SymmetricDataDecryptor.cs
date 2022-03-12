using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class SymmetricDataDecryptor<TData> : ValueReverseTransformerPipeline<TData>, IDecryptor<TData>
    {
        public SymmetricDataDecryptor(SymmetricDataEncryptor<TData> encryptor) : base(encryptor)
        {
            this.Encryptor = encryptor;
        }

        protected SymmetricDataEncryptor<TData> Encryptor { get; private set; }

        public TData Decrypt(Cipher<TData> cipherData)
        {
            return ReverseTransform(cipherData);
        }

        public string DecryptString(string cipher)
        {
            byte[] cipherData = Convert.FromBase64String(cipher);
            byte[] utf8 = this.Encryptor.AesByteTransformer.ReverseTransform(cipherData);

            return Encoding.UTF8.GetString(utf8);
        }

        public byte[] DecryptBytes(byte[] cipher)
        {
            return Encryptor.AesByteTransformer.ReverseTransform(cipher);
        }
    }
}
