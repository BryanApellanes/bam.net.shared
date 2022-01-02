using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IServerKeySet
    {
        string Identifier { get; }
        string RsaKey { get; }
        string AesKey { get; }
        string AesIV { get; }

        string Secret { get; }

        ISecretExchange GetSecretExchange();
    }
}
