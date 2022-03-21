using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class SymmetricContentDecryptor<TContent> : SymmetricDataDecryptor<TContent>, IContentDecryptor<TContent>
    {
        public SymmetricContentDecryptor(IAesKeySource aesKeysource) : base(new SymmetricDataEncryptor<TContent>(aesKeysource))
        {
        }

        public TContent DecryptContentCipher(ContentCipher<TContent> contentCipher)
        {
            return base.Decrypt(contentCipher);
        }
    }
}
