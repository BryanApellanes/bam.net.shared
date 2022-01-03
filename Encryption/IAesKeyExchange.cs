using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IAesKeyExchange
    {
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

        string PublicKey { get; set; }
        string AesKeyCipher { get; set; }
        string AesIVCipher { get; set; }
    }
}
