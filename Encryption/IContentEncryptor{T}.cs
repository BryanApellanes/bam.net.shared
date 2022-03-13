using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IContentEncryptor<TContent> : IEncryptor<TContent>
    {
        ContentCipher<TContent> GetContentCipher(TContent content);
    }
}
