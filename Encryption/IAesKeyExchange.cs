using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IAesKeyExchange
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets or sets the sender.  This is the host name of the client that 
        /// generated the aes key.
        /// </summary>
        string ClientHostName { get; set; }

        /// <summary>
        /// Gets or sets the receiver.  This is the host name of the server that has
        /// the private key used to decrypt the aes key.
        /// </summary>
        string ServerHostName { get; set; }

        /// <summary>
        /// Gets or sets the pem encoded rsa public key.
        /// </summary>
        string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the aes key encrypted with the public key.
        /// </summary>
        string AesKeyCipher { get; set; }

        /// <summary>
        /// Gets or sets the aes initialization vector encrypted with the public key.
        /// </summary>
        string AesIVCipher { get; set; }
    }
}
