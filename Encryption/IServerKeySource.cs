using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace bam.net.shared.Encryption
{
    public interface IServerKeySource : IAesKeySource, IRsaKeySource
    {
    }
}
