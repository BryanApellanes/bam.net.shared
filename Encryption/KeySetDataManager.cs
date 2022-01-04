using Bam.Net.Data;
using Bam.Net.Data.Repositories;
using Bam.Net.Encryption.Data;
using Bam.Net.Encryption.Data.Dao.Repository;
using Bam.Net.Services;
using Org.BouncyCastle.Crypto;
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
            this.ApplicationNameProvider = ProcessApplicationNameProvider.Current;
        }

        public KeySetDataManager(Database database)
        {
            this.EncryptionDataRepository = new EncryptionDataRepository() { Database = database };
            this.ApplicationNameProvider = ProcessApplicationNameProvider.Current;
        }

        public KeySetDataManager(EncryptionDataRepository repository)
        {
            this.EncryptionDataRepository = repository;
        }

        [Inject]
        public EncryptionDataRepository EncryptionDataRepository { get; }

        [Inject]
        public IApplicationNameProvider ApplicationNameProvider { get; set; }

        public async Task<IServerKeySet> CreateServerKeySetAsync(string clientHostName)
        {
            ServerKeySet serverKeySet = new ServerKeySet()
            {
                ApplicationName = ApplicationNameProvider.GetApplicationName(),
                ClientHostName = clientHostName
            };

            return await EncryptionDataRepository.SaveAsync(serverKeySet);
        }

        public async Task<IClientKeySet> CreateClientKeySetForServerKeySetAsync(IServerKeySet serverKeySet)
        {
            ClientKeySet clientKeySet = new ClientKeySet(false);
            clientKeySet.ServerHostName = serverKeySet.ServerHostName;
            clientKeySet.ClientHostName = serverKeySet.ClientHostName;
            clientKeySet.PublicKey = serverKeySet.GetAsymmetricKeys().PublicKeyToPem();
            
            return await EncryptionDataRepository.SaveAsync(clientKeySet);
        }

        public Task<IClientKeySet> SaveClientKeySet(IClientKeySet clientKeySet)
        {
            //ClientKeySet clientKeySet = new ClientKeySet
            throw new NotImplementedException();
        }

        public Task<IAesKeyExchange> CreateAesKeyExchangeAsync(IClientKeySet clientKeySet)
        {
            return Task.FromResult(clientKeySet.GetKeyExchange());
        }

        public async Task<IServerKeySet> SetServerAesKeyAsync(IAesKeyExchange keyExchange)
        {
            ServerKeySet serverKeySet = EncryptionDataRepository.OneServerKeySetWhere(query => query.Identifier == keyExchange.Identifier);
            AsymmetricCipherKeyPair rsaKeyPair = serverKeySet.GetAsymmetricKeys();
            serverKeySet.AesKey = keyExchange.AesKeyCipher.DecryptWithPrivateKey(rsaKeyPair);
            serverKeySet.AesIV = keyExchange.AesIVCipher.DecryptWithPrivateKey(rsaKeyPair);
            return await EncryptionDataRepository.SaveAsync(serverKeySet);
        }

        public Task<IServerKeySet> RetrieveServerKeySetForPublicKeyAsync(string publicKey)
        {
            string identifier = KeySet.GetIdentifier(publicKey);
            return RetrieveServerKeySetAsync(identifier);
        }

        public Task<IServerKeySet> RetrieveServerKeySetAsync(string identifier)
        {
            ServerKeySet serverKeySet = EncryptionDataRepository.OneServerKeySetWhere(query => query.Identifier == identifier);
            return Task.FromResult((IServerKeySet)serverKeySet);
        }

        public Task<ISecretExchange> GetSecretExchangeAsync(IServerKeySet serverKeys)
        {
            throw new NotImplementedException();
        }

        public Task<IClientKeySet> RetrieveClientKeySetForPublicKeyAsync(string publicKey)
        {
            throw new NotImplementedException();
        }

        public Task<IClientKeySet> RetrieveClientKeySetAsync(string identifier)
        {
            throw new NotImplementedException();
        }
    }
}
