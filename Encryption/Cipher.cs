using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class Cipher<TData> : Cipher
    {
        public static implicit operator byte[](Cipher<TData> cipher)
        {
            return cipher.Data;
        }

        public static implicit operator Cipher<TData>(byte[] data)
        {
            return new Cipher<TData>() { Data = data };
        }

        public static implicit operator string(Cipher<TData> cipher)
        {
            return cipher.Data.ToBase64();
        }

        public static implicit operator Cipher<TData>(string data)
        {
            return new Cipher<TData>() { Data = data.FromBase64() };
        }
    }

    public class Cipher
    {
        public static implicit operator byte[](Cipher cipher)
        {
            return cipher.Data;
        }

        public static implicit operator Cipher(byte[] data)
        {
            return new Cipher { Data = data };
        }

        public static implicit operator string(Cipher cipher)
        {
            return cipher.Data.ToBase64();
        }

        public static implicit operator Cipher(string data)
        {
            return new Cipher { Data = data.FromBase64() };
        }

        public byte[] Data { get; set; }
    }
}
