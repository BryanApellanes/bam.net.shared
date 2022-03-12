using Bam.Net.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Bam.Net.Encryption.Data
{
    /// <summary>
    /// Represents a key set for the current process or host to use
    /// in communication as the client.
    /// </summary>
    public class ClientKeySet : KeyedAuditRepoData, IApplicationKeySet, IClientKeySet, IAesKeySource, ICommunicationKeySet
    {
        public ClientKeySet() 
        {
            this.MachineName = Environment.MachineName;
            this.ClientHostName = Dns.GetHostName();
        }

        public ClientKeySet(bool initialize): this()
        {
            if (initialize)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Gets or sets the name of the machine that instantiated this keyset.
        /// </summary>
        [CompositeKey]
        public string MachineName { get; set; }

        /// <inheritdoc />
        [CompositeKey]
        public string ClientHostName { get; set; }

        /// <inheritdoc />
        [CompositeKey]
        public string ServerHostName { get; set; }

        string _publicKey;
        /// <inheritdoc />
        [CompositeKey]
        public string PublicKey
        {
            get
            {
                return _publicKey;
            }
            set
            {
                _publicKey = value;
                SetIdentifier();
            }
        }

        /// <inheritdoc />
        public string Identifier { get; set; }

        /// <inheritdoc />
        public string AesKey { get; set; }

        /// <inheritdoc />
        public string AesIV { get; set; }

        /// <inheritdoc />
        public string ApplicationName { get; set; }

        public string Secret { get; set; }

        /// <summary>
        /// Sets the aes key and initialization vector if they are not yet set.
        /// </summary>
        public void Initialize()
        {
            EnsureAesKey();
        }

        /// <inheritdoc />
        public AesKeyVectorPair GetAesKey()
        {
            EnsureAesKey();
            return new AesKeyVectorPair(AesKey, AesIV);
        }

        /// <summary>
        /// Gets a vlaue
        /// </summary>
        /// <returns></returns>
        public bool GetIsInitialized()
        {
            return !string.IsNullOrEmpty(AesKey) && !string.IsNullOrEmpty(AesIV);
        }

        public IAesKeyExchange GetKeyExchange()
        {
            EnsureAesKey();
            return new AesKeyExchange(this);
        }

        protected void EnsureAesKey()
        {
            if (!GetIsInitialized())
            {
                SetAesKey();
            }
        }

        protected void SetAesKey()
        {
            AesKeyVectorPair aesKeyVectorPair = new AesKeyVectorPair();
            AesKey = aesKeyVectorPair.Key;
            AesIV = aesKeyVectorPair.IV;
        }

        protected void SetIdentifier()
        {
            if (!string.IsNullOrEmpty(PublicKey))
            {
                this.Identifier = KeySet.GetIdentifier(PublicKey);
            }
        }

        public string Encrypt(string value)
        {
            return Aes.Encrypt(value, GetAesKey());
        }

        public string Decrypt(string base64EncodedCipher)
        {
            return Aes.Decrypt(base64EncodedCipher, GetAesKey());
        }

        public RsaPublicKey GetRsaPublicKey()
        {
            return new RsaPublicKey(PublicKey);
        }

        public string PublicKeyEncrypt(string value)
        {
            return value.EncryptWithPublicKey(PublicKey);
        }
    }
}
