using Bam.Net.Data;
using Bam.Net.Encryption.Data.Dao.Repository;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Encryption
{
    public class ClientKeySetDataManager : IClientKeySetDataManager
    {
        public ClientKeySetDataManager()
        {
            this.EncryptionDataRepository = new EncryptionDataRepository();
            this.ApplicationNameProvider = ProcessApplicationNameProvider.Current;
        }

        public ClientKeySetDataManager(Database database)
        {
            this.EncryptionDataRepository = new EncryptionDataRepository() { Database = database };
            this.ApplicationNameProvider = ProcessApplicationNameProvider.Current;
        }

        [Inject]
        public EncryptionDataRepository EncryptionDataRepository { get; set; }

        [Inject]
        public IApplicationNameProvider ApplicationNameProvider { get; set; }

        public Task<IAesKeyExchange> CreateAesKeyExchangeAsync(IClientKeySet clientKeySet)
        {
            return Task.FromResult(clientKeySet.GetKeyExchange());
        }

        public Task<IClientKeySet> RetrieveClientKeySetAsync(string identifier)
        {
            throw new NotImplementedException();
        }

        public Task<IClientKeySet> RetrieveClientKeySetForPublicKeyAsync(string publicKey)
        {
            throw new NotImplementedException();
        }

        public Task<IClientKeySet> SaveClientKeySet(IClientKeySet clientKeySet)
        {
            throw new NotImplementedException();
        }
    }
}
