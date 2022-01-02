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

        [CompositeKey]
        public string PublicKey { get; set; }

        public string Identifier { get; set; }

        public string AesKey { get; set; }

        public string AesIV { get; set; }
        public string ApplicationName { get; set; }

        public AesKeyVectorPair GetAesKey()
        {
            return new AesKeyVectorPair(AesKey, AesIV);
        }

        public IAesKeyExchange GetKeyExchange()
        {
            return new AesKeyExchange(this);
        }
    }
}
