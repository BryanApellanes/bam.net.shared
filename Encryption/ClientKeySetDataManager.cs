using Bam.Net.Data;
using Bam.Net.Encryption.Data;
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

        public async Task<IAesKeyExchange> CreateAesKeyExchangeAsync(IClientKeySet clientKeySet)
        {
            if (!clientKeySet.GetIsInitialized())
            {
                clientKeySet.Initialize();
            }

            if (!(await RetrieveClientKeySetAsync(clientKeySet.Identifier) is ClientKeySet existingClientKeySet))
            {
                ClientKeySet copy = clientKeySet.CopyAsNew<ClientKeySet>();

                clientKeySet = await SaveClientKeySetAsync(copy);
            }
            else
            {
                existingClientKeySet.PublicKey = clientKeySet.PublicKey;
                existingClientKeySet.AesKey = clientKeySet.AesKey;
                existingClientKeySet.AesIV = clientKeySet.AesIV;
                clientKeySet = await SaveClientKeySetAsync(existingClientKeySet);
            }

            return clientKeySet.GetKeyExchange();
        }

        public Task<IClientKeySet> RetrieveClientKeySetAsync(string identifier)
        {
            return Task.FromResult((IClientKeySet)EncryptionDataRepository.OneClientKeySetWhere(query => query.Identifier == identifier));
        }

        public Task<IClientKeySet> RetrieveClientKeySetForPublicKeyAsync(string publicKey)
        {
            throw new NotImplementedException();
        }

        public async Task<IClientKeySet> SaveClientKeySetAsync(IClientKeySet clientKeySet)
        {
            ClientKeySet toSave = clientKeySet.CopyAs<ClientKeySet>();
            return await EncryptionDataRepository.SaveAsync(toSave);
        }
    }
}
