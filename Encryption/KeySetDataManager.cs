using Bam.Net.Data;
using Bam.Net.Data.Repositories;
using Bam.Net.Encryption.Data;
using Bam.Net.Encryption.Data.Dao.Repository;
//using Bam.Net.Encryption.Data.Dao.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Encryption
{
    public class KeySetDataManager : IKeySetDataManager
    {
        public KeySetDataManager()
        {
            this.EncryptionDataRepository = new EncryptionDataRepository();
        }

        public KeySetDataManager(Database database)
        {
            this.EncryptionDataRepository = new EncryptionDataRepository() { Database = database };
        }

        public KeySetDataManager(EncryptionDataRepository repository)
        {
            this.EncryptionDataRepository = repository;
        }

        public EncryptionDataRepository EncryptionDataRepository { get; }

        public async Task<IClientKeySet> CreateClientKeySetAsync(string serverHostName)
        {
            ClientKeySet clientKeySet = new ClientKeySet() { ServerHostName = serverHostName };
            return await EncryptionDataRepository.SaveAsync(clientKeySet);
        }

        public async Task<IAesKeyExchange> CreateKeyExchangeAsync(string serverHostName)
        {
            IClientKeySet clientKeySet = await CreateClientKeySetAsync(serverHostName);
            return clientKeySet.GetKeyExchange();
        }

        public async Task<IServerKeySet> CreateServerKeySetAsync(string clientHostName)
        {
            ServerKeySet serverKeySet = new ServerKeySet() { ClientHostName = clientHostName };
            return await EncryptionDataRepository.SaveAsync(serverKeySet);
        }

        public async Task<ISecretExchange> CreateSecretExchangeAsync(string clientHostName)
        {
            IServerKeySet serverKeySet = await CreateServerKeySetAsync(clientHostName);
            return serverKeySet.GetSecretExchange();
        }

        public Task<IAesKeyExchange> GetKeyExchangeAsync(IClientKeySet serverKeys)
        {
            return Task.FromResult(serverKeys.GetKeyExchange());
        }

        public Task<IAesKeyExchange> GetKeyExchangeAsync(string clientKeySetIdentifier)
        {
            throw new NotImplementedException();
            //ClientKeySet clientKeySet = EncryptionDataRepository.GetOneClientKeySetWhere(query => query.Identifier == clientKeySetIdentifier);
            //return GetKeyExchangeAsync(clientKeySet);
        }

        public Task<ISecretExchange> GetSecretExchangeAsync(IServerKeySet serverKeys)
        {
            return Task.FromResult(serverKeys.GetSecretExchange());
        }

        public Task<ISecretExchange> GetSecretExchangeAsync(string serverKeySetIdentifier)
        {
            throw new NotImplementedException();
            //ServerKeySet serverKeySet = EncryptionDataRepository.GetOneServerKeySetWhere(query => query.Identifier == serverKeySetIdentifier);
            //return GetSecretExchangeAsync(serverKeySet);
        }
    }
}
