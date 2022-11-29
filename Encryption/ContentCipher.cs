using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public abstract class ContentCipher : Cipher, IContentCipher
    {
        public static implicit operator byte[](ContentCipher cipher)
        {
            return cipher.Data;
        }

        public static implicit operator string(ContentCipher cipher)
        {
            return cipher.Data.ToBase64();
        }
                

        public string ContentType { get; protected set; }
    }
}
