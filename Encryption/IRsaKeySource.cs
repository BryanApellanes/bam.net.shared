using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IRsaKeySource : IRsaPublicKeySource
    {
        RsaPublicPrivateKeyPair GetRsaKey();
    }
}
