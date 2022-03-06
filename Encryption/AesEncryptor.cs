using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesEncryptor : IEncryptor
    {
        public AesEncryptor(IAesKeySource keySource)
        {
            this.KeyProvider = () => keySource.GetAesKey();
        }
        public AesEncryptor(Func<AesKeyVectorPair> keyProvider)
        {
            this.KeyProvider = keyProvider;
        }

        public AesEncryptor(AesKeyVectorPair aesKeyVectorPair)
        {
            this.KeyProvider = () => aesKeyVectorPair;
        }

        public Func<AesKeyVectorPair> KeyProvider { get; set; }

        public string EncryptString(string plainData)
        {
            AesKeyVectorPair aesKeyVectorPair = KeyProvider();
            return aesKeyVectorPair.Encrypt(plainData);
        }

        public byte[] EncryptBytes(byte[] plainData)
        {
            AesKeyVectorPair aesKeyVectorPair = KeyProvider();
            return aesKeyVectorPair.EncryptBytes(plainData);            
        }
    }
}
