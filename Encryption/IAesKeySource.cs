using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IAesKeySource
    {
        /// <summary>
        /// Get an aes key.
        /// </summary>
        /// <returns>AesKeyVectorPair</returns>
        AesKeyVectorPair GetAesKey();
    }
}
