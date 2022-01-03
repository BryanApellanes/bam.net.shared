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

        /// <summary>
        /// Gets or sets the dns hostname name of the machine that instantiated this keyset.
        /// </summary>
        [CompositeKey]
        public string ClientHostName { get; set; }

        [CompositeKey]
        public string ServerHostName { get; set; }

        string _publicKey;
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

        public string Identifier { get; set; }

        public string AesKey { get; set; }

        public string AesIV { get; set; }
        public string ApplicationName { get; set; }

        public void Initialize()
        {
            EnsureAesKey();
        }

        public AesKeyVectorPair GetAesKey()
        {
            EnsureAesKey();
            return new AesKeyVectorPair(AesKey, AesIV);
        }

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
    }
}
