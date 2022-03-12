using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IServerKeySet : IApplicationKeySet, IKeySet, ICommunicationKeySet
    {
        string Identifier { get; }

        string Secret { get; }

        bool GetIsRsaInitialized();
        bool GetIsAesInitialized();

        void InitializeRsaKey();
        void InitializeAesKey();

        ISecretExchange GetSecretExchange();

        string PrivateKeyDecrypt(string value);
    }
}
