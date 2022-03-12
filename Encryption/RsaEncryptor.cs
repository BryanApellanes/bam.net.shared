using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaEncryptor : IEncryptor
    {
        public RsaEncryptor(IRsaKeySource rsaKeySource)
        {
            this.KeyProvider = () => rsaKeySource.GetRsaKey();
        }

        public RsaEncryptor(Func<RsaPublicPrivateKeyPair> keyProvider)
        {
            this.KeyProvider = keyProvider;
        }

        public RsaEncryptor(RsaPublicPrivateKeyPair rsaPublicPrivateKeyPair)
        {
            this.KeyProvider = () => rsaPublicPrivateKeyPair;
        }

        public Func<RsaPublicPrivateKeyPair> KeyProvider { get; set; }
        public string EncryptString(string plainData)
        {
            RsaPublicPrivateKeyPair rsaPublicPrivateKeyPair = KeyProvider();            
            return rsaPublicPrivateKeyPair.Encrypt(plainData);
        }

        public byte[] EncryptBytes(byte[] plainData)
        {
            RsaPublicPrivateKeyPair rsaPublicPrivateKeyPair = KeyProvider();
            return rsaPublicPrivateKeyPair.EncryptBytes(plainData);
        }
    }
}
