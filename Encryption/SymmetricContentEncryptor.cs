using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class SymmetricContentEncryptor<TContent> : SymmetricDataEncryptor<TContent>, IContentEncryptor<TContent>
    {
        public SymmetricContentEncryptor(IAesKeySource aesKeySource) : base(aesKeySource)
        {
        }

        public ContentCipher<TContent> GetContentCipher(TContent content)
        {
            return new SymmetricContentCipher<TContent>(Encrypt(content));
        }
    }
}
