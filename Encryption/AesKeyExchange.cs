using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesKeyExchange : IAesKeyExchange
    {
        public AesKeyExchange(IClientKeySet keySet)
        {
            this.PublicKey = keySet.PublicKey;
            this.AesKeyCipher = keySet.AesKey.EncryptWithPublicKey(keySet.PublicKey);
            this.AesIVCipher = keySet.AesKey.EncryptWithPublicKey(keySet.PublicKey);
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

        public string Client
        {
            get;
            set;
        }

        public string Server
        {
            get;
            set;
        }
    }
}
