using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IContentDecryptor<TContent> : IDecryptor<TContent>
    {
        TContent DecryptContentCipher(ContentCipher<TContent> contentCipher);
    }
}
