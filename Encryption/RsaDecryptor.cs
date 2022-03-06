using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaDecryptor : IDecryptor
    {
        public RsaDecryptor(IRsaKeySource rsaKeySource)
        {
            this.KeyProvider = () => rsaKeySource.GetRsaKey();
        }

        public RsaDecryptor(Func<RsaPublicPrivateKeyPair> keyProvider)
        {
            this.KeyProvider = keyProvider;
        }

        public RsaDecryptor(RsaPublicPrivateKeyPair rsaPublicPrivateKeyPair)
        {
            this.KeyProvider = () => rsaPublicPrivateKeyPair;
        }

        public Func<RsaPublicPrivateKeyPair> KeyProvider { get; set; }

        public string DecryptString(string cipher)
        {
            RsaPublicPrivateKeyPair rsaPublicPrivateKeyPair = KeyProvider();
            return rsaPublicPrivateKeyPair.Decrypt(cipher);
        }

        public byte[] DecryptBytes(byte[] cipher)
        {
            RsaPublicPrivateKeyPair rsaPublicPrivateKeyPair = KeyProvider();
            return rsaPublicPrivateKeyPair.DecryptBytes(cipher);
        }
    }
}
