using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AsymmetricContentCipher<TContent> : ContentCipher<TContent>
    {
        public AsymmetricContentCipher(byte[] data)
        {
            this.Data = data;
            this.ContentType = MediaTypes.AsymmetricCipher;
        }
    }
}
