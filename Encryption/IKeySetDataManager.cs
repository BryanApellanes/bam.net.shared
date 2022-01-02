//using Bam.Net.Encryption.Data.Dao.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Encryption
{
    public interface IKeySetDataManager
    { 
        //EncryptionDataRepository EncryptionDataRepository { get; }

        Task<IServerKeySet> CreateServerKeySetAsync(string clientHostName);
        
        Task<IClientKeySet> CreateClientKeySetAsync(string serverHostName);

        Task<IAesKeyExchange> CreateKeyExchangeAsync(string serverHostName);

        Task<IAesKeyExchange> GetKeyExchangeAsync(IClientKeySet clientKeySet);

        Task<IAesKeyExchange> GetKeyExchangeAsync(string clientKeySetIdentifier);

        Task<ISecretExchange> CreateSecretExchangeAsync(string clientHostName);

        Task<ISecretExchange> GetSecretExchangeAsync(IServerKeySet serverKeys);

        Task<ISecretExchange> GetSecretExchangeAsync(string serverKeySetIdentifier);
    }
}
