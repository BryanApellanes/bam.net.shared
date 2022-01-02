using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IClientKeySet : ICommunicationKeySet
    {
        /// <summary>
        /// Gets a value that uniquely identifies this client key set.
        /// </summary>
        string Identifier { get; }

        string PublicKey { get; }
        string AesKey { get; }
        string AesIV { get; }

        IAesKeyExchange GetKeyExchange();
    }
}
