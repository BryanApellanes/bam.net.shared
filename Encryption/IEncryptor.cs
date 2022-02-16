using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IEncryptor
    {
        string Encrypt(string plainData);
        byte[] EncryptBytes(byte[] plainData);
    }
}
