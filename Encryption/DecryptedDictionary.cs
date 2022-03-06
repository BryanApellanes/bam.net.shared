using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class DecryptedDictionary : IDecryptedDictionary
    {
        private Dictionary<string, string> _unencrypted;
        private Dictionary<string, string> _encrypted;
        public DecryptedDictionary(IDecryptor decryptor)
        {
            Args.ThrowIfNull(decryptor);
            this.Decryptor = decryptor;
            this._unencrypted = new Dictionary<string, string>();
            this._encrypted = new Dictionary<string, string>();
        }

        public static DecryptedDictionary FromString(IDecryptor decryptor, string encryptedDictionaryString)
        {
            DecryptedDictionary decryptedDictionary = new DecryptedDictionary(decryptor);
            string[] lines = encryptedDictionaryString.DelimitSplit("\r", "\n");
            foreach(string line in lines)
            {
                string[] keyValue = line.DelimitSplit(".");
                decryptedDictionary.Add(keyValue[0], keyValue[1]);
            }
            return decryptedDictionary;
        }

        public string this[string key] 
        {
            get
            {
                if (_unencrypted.ContainsKey(key))
                {
                    return _unencrypted[key];
                }
                else if(_encrypted.ContainsKey(key))
                {
                    return _encrypted[key];
                }
                return null;
            }
            set
            {
                if (_encrypted.ContainsKey(key))
                {
                    _encrypted[key] = value;
                }
                else
                {
                    _encrypted.Add(key, value);
                }

                string decryptedKey = this.Decryptor.DecryptString(key);
                string decryptedValue = this.Decryptor.DecryptString(value);
                if(!_unencrypted.ContainsKey(decryptedKey))
                {
                    _unencrypted.Add(decryptedKey, decryptedValue);
                }
                else
                {
                    _unencrypted[decryptedKey] = decryptedValue;
                }
            }
        }

        public IDecryptor Decryptor
        {
            get;
            private set;
        }

        public ICollection<string> Keys => _unencrypted.Keys;

        public ICollection<string> Values => _unencrypted.Values;

        public int Count => _unencrypted.Count;

        public void Add(string key, string value)
        {
            this[key] = value;
        }

        public void Add(System.Collections.Generic.KeyValuePair<string, string> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this._encrypted.Clear();
            this._unencrypted.Clear();
        }

        public bool Contains(System.Collections.Generic.KeyValuePair<string, string> item)
        {
            if (this._encrypted.ContainsKey(item.Key))
            {
                return this._encrypted[item.Key] == item.Value;
            }

            if (this._unencrypted.ContainsKey(item.Key))
            {
                return this._unencrypted[item.Key] == item.Value;
            }

            return false;
        }

        public bool ContainsKey(string key)
        {
            return this._encrypted.ContainsKey(key) || this._unencrypted.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            if (this._encrypted.ContainsKey(key))
            {
                return this._encrypted.Remove(key);
            }
            if (this._unencrypted.ContainsKey(key))
            {
                return this._unencrypted.Remove(key);
            }

            return false;
        }
    }
}
