using Bam.Net.Encryption.Data.Dao.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Encryption
{
    public interface IKeySetDataManager
    { 
        EncryptionDataRepository EncryptionDataRepository { get; }

        /// <summary>
        /// Creates a server key set for the current process to act as a server
        /// to the the specified client.
        /// </summary>
        /// <param name="clientHostName"></param>
        /// <returns></returns>
        Task<IServerKeySet> CreateServerKeySetAsync(string clientHostName); // server side : startsession

        /// <summary>
        /// Create a client key set for the specified server key set to be sent
        /// to the client.
        /// </summary>
        /// <param name="serverKeySet"></param>
        /// <returns></returns>
        Task<IClientKeySet> CreateClientKeySetForServerKeySetAsync(IServerKeySet serverKeySet); // server side : send public key no aes key yet
        
        /// <summary>
        /// Create an aes key exchange for the specified client key set.
        /// </summary>
        /// <param name="clientKeySet"></param>
        /// <returns></returns>
        Task<IAesKeyExchange> CreateAesKeyExchangeAsync(IClientKeySet clientKeySet); // client side: set the aes key and send exchange

        /// <summary>
        /// Set the server key set aes key and iv using the specified key exchange.
        /// </summary>
        /// <param name="keyExchange"></param>
        /// <returns></returns>
        Task<IServerKeySet> SetServerAesKeyAsync(IAesKeyExchange keyExchange); // server side: retrieve the server key set by the public key and set the aes key
        
        Task<IServerKeySet> RetrieveServerKeySetForPublicKeyAsync(string publicKey); // server side: retrieve server key set to enable secret exchange

        Task<IServerKeySet> RetrieveServerKeySet(string identifier);

        Task<ISecretExchange> GetSecretExchangeAsync(IServerKeySet serverKeys); // server side: one time secret exchange

        Task<IClientKeySet> RetrieveClientKeySetForPublicKeyAsync(string publicKey);

        Task<IClientKeySet> RetrieveClientKeySet(string identifier);
    }
}
