/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Encryption;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Engines;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class ClientSessionInfo : IClientKeySource 
    {
        public ClientSessionInfo()
        {
            this.ValidationAlgorithm = HashAlgorithms.SHA256;
        }

        public string ServerHostName
        {
            get;
            set;
        }

        /// <summary>
        /// The value of the session cookie.
        /// </summary>
        public string ClientSessionIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the server Rsa public key of the current session as a Pem string.
        /// </summary>
        public string PublicKey
        {
            get;
            set;
        }

        public HashAlgorithms ValidationAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// The key for the current session.
        /// </summary>
        protected internal string AesKey
        {
            get;
            set;
        }

        /// <summary>
        /// The initialization vector for the current session
        /// </summary>
        protected internal string AesIV
        {
            get;
            set;
        }

        public SetSessionKeyRequest CreateSetSessionKeyRequest(Encoding encoding = null)
        {
            Args.ThrowIfNullOrEmpty(PublicKey, nameof(PublicKey));

            encoding = encoding ?? Encoding.UTF8;
            AesKeyVectorPair kvp = InitializeSessionKey();
            string keyCipher = kvp.Key.EncryptWithPublicKey(PublicKey, encoding);
            string keyHash = kvp.Key.HashHexString(ValidationAlgorithm, encoding);
            string keyHashCipher = keyHash.EncryptWithPublicKey(PublicKey, encoding);
            string ivCipher = kvp.IV.EncryptWithPublicKey(PublicKey, encoding);
            string ivHash = kvp.IV.HashHexString(ValidationAlgorithm, encoding);
            string ivHashCipher = ivHash.EncryptWithPublicKey(PublicKey, encoding);

            return new SetSessionKeyRequest(keyCipher, keyHashCipher, ivCipher, ivHashCipher);
        }

        public override bool Equals(object obj)
        {
            if (obj is ClientSessionInfo info)
            {
                return info.ClientSessionIdentifier.Equals(ClientSessionIdentifier) && info.PublicKey.Equals(PublicKey);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.GetHashCode(ClientSessionIdentifier, PublicKey);
        }

        public override string ToString()
        {
            return $"{nameof(ClientSessionIdentifier)}={ClientSessionIdentifier};{nameof(PublicKey)}={PublicKey}";
        }

        protected internal AesKeyVectorPair InitializeSessionKey()
        {
            AesKeyVectorPair kvp = new AesKeyVectorPair();
            AesKey = kvp.Key;
            AesIV = kvp.IV;
            return kvp;
        }

        public AesKeyVectorPair GetAesKey()
        {
            return new AesKeyVectorPair(AesKey, AesIV);
        }

        public RsaPublicKey GetRsaPublicKey()
        {
            return new RsaPublicKey(PublicKey);
        }
    }
}
