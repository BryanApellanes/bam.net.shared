using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IClientKeySet
    {
        /// <summary>
        /// Gets a value that uniquely identifies this client key set.
        /// </summary>
        string Identifier { get; }
        string ServerHostName { get; }

        string PublicKey { get; }
        string AesKey { get; }
        string AesIV { get; }

        IAesKeyExchange GetKeyExchange();
    }
}
