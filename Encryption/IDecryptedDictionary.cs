using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IDecryptedDictionary 
    {
        IDecryptor Decryptor { get; }

        string this[string key] { get; set; }

        ICollection<string> Keys { get; }
        ICollection<string> Values { get; }

        int Count { get; }
        void Add(string key, string value);
        void Add(System.Collections.Generic.KeyValuePair<string, string> item);
        void Clear();
        bool Contains(System.Collections.Generic.KeyValuePair<string, string> item);
        bool ContainsKey(string key);
        bool Remove(string key);
    }
}
