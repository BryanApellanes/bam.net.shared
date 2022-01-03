using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IClientKeySet : ICommunicationKeySet
    {
        /// <summary>
        /// Gets a value that uniquely identifies this client key set by the hash of the public key.
        /// </summary>
        string Identifier { get; }

        string PublicKey { get; }
        string AesKey { get; }
        string AesIV { get; }

        /// <summary>
        /// Gets a value indicating whether the AesKey and AesIV are not blank.
        /// </summary>
        /// <returns></returns>
        bool GetIsInitialized();

        IAesKeyExchange GetKeyExchange();
    }
}
