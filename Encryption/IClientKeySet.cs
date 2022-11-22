using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IClientKeySet : ICommunicationKeySet, IRsaPublicKeySource, IAesKeySource
    {
        /// <summary>
        /// Gets a value that uniquely identifies this client key set by the hash of the public key.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets or sets the pem encoded public key.
        /// </summary>
        string PublicKey { get; }

        /// <summary>
        /// Gets or sets the aes key.
        /// </summary>
        string AesKey { get; }

        /// <summary>
        /// Gets or sets the aes initialization vector.
        /// </summary>
        string AesIV { get; }

        string Secret { get; }

        string PublicKeyEncrypt(string value);
        string Encrypt(string value);

        string Decrypt(string base64EncodedValue);

        /// <summary>
        /// Gets a value indicating whether the aes key and initializaiton vector are not blank.
        /// </summary>
        /// <returns></returns>
        bool GetIsInitialized();

        /// <summary>
        /// Ensures that the aes key and initialization vector are set.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets an aes key exchange for the current client key set.
        /// </summary>
        /// <returns></returns>
        IAesKeyExchange GetKeyExchange();
    }
}
