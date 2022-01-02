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
    public class ClientKeySet : ApplicationKeySet, IClientKeySet
    {
        public ClientKeySet() : base(RsaKeyLength._2048, true)
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

        string _publicKeyPem;
        public string PublicKey
        {
            get
            {
                if (string.IsNullOrEmpty(_publicKeyPem))
                {
                    _publicKeyPem = RsaKey.FromPem().PublicKeyToPem();
                }
                return _publicKeyPem;
            }
        }

        public IAesKeyExchange GetKeyExchange()
        {
            return new KeyExchange(this);
        }
    }
}
