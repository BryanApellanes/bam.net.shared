using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class KeyExchange : IAesKeyExchange
    {
        public KeyExchange(IKeySet keySet)
        {
            AsymmetricCipherKeyPair asymmetricCipherKeyPair = keySet.GetAsymmetricKeys();
            this.PublicKey = asymmetricCipherKeyPair.PublicKeyToPem();
            this.AesKeyCipher = keySet.AesKey.EncryptWithPublicKey(asymmetricCipherKeyPair.Public);
            this.AesIVCipher = keySet.AesKey.EncryptWithPublicKey(asymmetricCipherKeyPair.Public);
        }

        public string PublicKey
        {
            get;
            set;
        }

        public string AesKeyCipher 
        {
            get;
            set;
        }

        public string AesIVCipher 
        {
            get;
            set;
        }

        public string Sender
        {
            get;
            set;
        }

        public string Receiver
        {
            get;
            set;
        }
    }
}
