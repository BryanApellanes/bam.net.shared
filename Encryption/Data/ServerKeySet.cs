using Bam.Net.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Bam.Net.Encryption.Data
{
    /// <summary>
    /// Represents a key set for the current process or host to use 
    /// in communication as the server.
    /// </summary>
    public class ServerKeySet : ApplicationKeySet, IServerKeySet
    {
        public ServerKeySet() : base(RsaKeyLength._2048, true)
        {
            this.MachineName = Environment.MachineName;
            this.ServerHostName = Dns.GetHostName();
        }

        /// <summary>
        /// Gets or sets the name of the machine that instantiated this keyset.
        /// </summary>
        [CompositeKey]
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the dns hostname of the machine that instantiated this keyset.
        /// </summary>
        [CompositeKey]
        public string ServerHostName { get; set; }

        [CompositeKey]
        public string ClientHostName { get; set; }

        public ISecretExchange GetSecretExchange()
        {
            // Only send if the current machine created this key set.
            if (MachineName.Equals(Environment.MachineName)) 
            {
                return new SecretExchange
                {
                    KeySetIdentifier = this.Identifier,
                    Sender = ServerHostName,
                    Receiver = ClientHostName,
                    SecretCipher = Encrypt(Secret)
                };
            }

            throw new InvalidOperationException($"The {nameof(ServerKeySet)} with identifier {Identifier} is not from the current machine ({Environment.MachineName})");
        }
    }
}
