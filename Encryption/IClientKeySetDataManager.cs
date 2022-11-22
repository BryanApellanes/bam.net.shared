using Bam.Net.Encryption.Data.Dao.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Encryption
{
    public interface IClientKeySetDataManager
    {
        EncryptionDataRepository EncryptionDataRepository { get; }

        IApplicationNameProvider ApplicationNameProvider { get; }

        Task<IClientKeySet> SaveClientKeySetAsync(IClientKeySet clientKeySet); // client side: save the client key set for future retrieval

        /// <summary>
        /// Create an aes key exchange for the specified client key set.
        /// </summary>
        /// <param name="clientKeySet"></param>
        /// <returns></returns>
        Task<IAesKeyExchange> CreateAesKeyExchangeAsync(IClientKeySet clientKeySet); // client side: set the aes key and send exchange

        Task<IClientKeySet> RetrieveClientKeySetForPublicKeyAsync(string publicKey); // client side

        Task<IClientKeySet> RetrieveClientKeySetAsync(string identifier); // client side

        Task<IClientKeySet> SetSecret(ISecretExchange secretExchange);
    }
}
