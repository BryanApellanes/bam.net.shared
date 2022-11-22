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
            this.Identifier = keySet.Identifier;
            this.PublicKey = keySet.PublicKey;
            this.AesKeyCipher = keySet.AesKey.EncryptWithPublicKey(keySet.PublicKey);
            this.AesIVCipher = keySet.AesIV.EncryptWithPublicKey(keySet.PublicKey);
            this.ClientHostName = keySet.ClientHostName;
            this.ServerHostName = keySet.ServerHostName;
        }

        /// <inheritdoc />
        public string Identifier
        {
            get;
        }

        /// <inheritdoc />
        public string PublicKey
        {
            get;
            set;
        }

        /// <inheritdoc />
        public string AesKeyCipher 
        {
            get;
            set;
        }

        /// <inheritdoc />
        public string AesIVCipher 
        {
            get;
            set;
        }

        /// <inheritdoc />
        public string ClientHostName
        {
            get;
            set;
        }

        /// <inheritdoc />
        public string ServerHostName
        {
            get;
            set;
        }
    }
}
