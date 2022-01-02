using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface ISecretExchange
    {
        /// <summary>
        /// Gets or sets the identifier for the aes key
        /// used to decrypt the secret.
        /// </summary>
        string KeySetIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the sender (server) host name.
        /// </summary>
        string Sender { get; set; }

        /// <summary>
        /// Gets or sets the receiver (client) host name.
        /// </summary>
        string Receiver { get; set; }

        /// <summary>
        /// Gets or sets the secret encrypted with the aes key 
        /// of the specified KeySetIdentifier.
        /// </summary>
        string SecretCipher { get; set; }
    }
}
