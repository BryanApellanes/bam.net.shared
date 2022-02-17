using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IEncryptor<TData>
    {
        byte[] Encrypt(TData data);

        IDecryptor<TData> GetDecryptor();
    }

    public interface IEncryptor
    {
        string Encrypt(string plainData);
        byte[] EncryptBytes(byte[] plainData);
    }
}
