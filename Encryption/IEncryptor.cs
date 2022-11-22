using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IEncryptor<TData> : IEncryptor
    {
        Cipher<TData> Encrypt(TData data);

        IDecryptor<TData> GetDecryptor();
    }

    public interface IEncryptor
    {
        string EncryptString(string plainData);
        byte[] EncryptBytes(byte[] plainData);
    }
}
