﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IRsaKeySource
    {
        RsaPublicPrivateKeyPair GetRsaKey();
    }
}
