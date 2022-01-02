using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IAesKeyExchange
    {
        /// <summary>
        /// Gets or sets the sender.  This should be the host name of the client that 
        /// generated the aes key.
        /// </summary>
        string Sender { get; set; }

        /// <summary>
        /// Gets or sets the receiver.  This should be the host name of the server that has
        /// the private key used to decrypt the aes key.
        /// </summary>
        string Receiver { get; set; }

        string PublicKey { get; set; }
        string AesKeyCipher { get; set; }
        string AesIVCipher { get; set; }
    }
}
