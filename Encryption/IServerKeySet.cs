using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IServerKeySet : IApplicationKeySet, IKeySet, ICommunicationKeySet
    {
        string Identifier { get; }

        string Secret { get; }

        ISecretExchange GetSecretExchange();
    }
}
