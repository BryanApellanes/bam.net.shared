using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public abstract class ContentCipher<TContent> : Cipher<TContent>, IContentCipher
    {
        public static implicit operator byte[](ContentCipher<TContent> cipher)
        {
            return cipher.Data;
        }

        public static implicit operator string(ContentCipher<TContent> cipher)
        {
            return cipher.Data.ToBase64();
        }

        public string ContentType { get; protected set; }
    }
}
