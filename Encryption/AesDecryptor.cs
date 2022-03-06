using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesDecryptor : IDecryptor
    {
        public AesDecryptor(IAesKeySource aesKeySource)
        {
            this.KeyProvider = () => aesKeySource.GetAesKey();
        }

        public AesDecryptor(Func<AesKeyVectorPair> keyProvider)
        {
            this.KeyProvider = keyProvider;
        }

        public AesDecryptor(AesKeyVectorPair aesKeyVectorPair)
        {
            this.KeyProvider = () => aesKeyVectorPair;
        }

        public Func<AesKeyVectorPair> KeyProvider { get; set; }

        public string DecryptString(string cipher)
        {
            AesKeyVectorPair aesKeyVectorPair = KeyProvider();
            return aesKeyVectorPair.Decrypt(cipher);
        }

        public byte[] DecryptBytes(byte[] cipher)
        {
            AesKeyVectorPair aesKeyVectorPair = KeyProvider();
            return aesKeyVectorPair.DecryptBytes(cipher);
        }
    }
}
