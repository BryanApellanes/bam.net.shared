using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    /// <summary>
    /// A specialized data encryptor that produces content ciphers.
    /// </summary>
    /// <typeparam name="TContent"></typeparam>
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
