using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IDecryptor
    {
        string Decrypt(string cipher);
        byte[] DecryptBytes(byte[] cipher);
    }
}
