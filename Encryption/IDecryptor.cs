using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IDecryptor<TData> : IDecryptor
    {
        TData Decrypt(Cipher<TData> cipherData);
    }

    public interface IDecryptor
    {
        string DecryptString(string cipher);
        byte[] DecryptBytes(byte[] cipher);
    }
}
