using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class SecretExchange : ISecretExchange
    {
        /// <inheritdoc />
        public string KeySetIdentifier { get; set; }

        /// <inheritdoc />
        public string Sender { get; set; }
        
        /// <inheritdoc />
        public string Receiver { get; set; }

        /// <inheritdoc />
        public string SecretCipher { get; set; }
    }
}
