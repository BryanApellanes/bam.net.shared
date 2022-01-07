using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    /// <summary>
    /// Provides a way for a server to securely share a secret with a client.
    /// </summary>
    public interface ISecretExchange
    {
        /// <summary>
        /// Gets or sets the identifier for the aes key
        /// used to decrypt the secret.
        /// </summary>
        string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the server host name who is the sender of the secret.
        /// </summary>
        string ServerHostName { get; set; }

        /// <summary>
        /// Gets or sets the client host name who is the receiver of the secret.
        /// </summary>
        string ClientHostName { get; set; }

        /// <summary>
        /// Gets or sets the secret encrypted with the aes key 
        /// of the keyset with the same identifier.
        /// </summary>
        string SecretCipher { get; set; }
    }
}
