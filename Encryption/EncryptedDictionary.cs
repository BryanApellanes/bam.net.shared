using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class EncryptedDictionary : IEncryptedDictionary
    {
        private Dictionary<string, string> _unencrypted;
        private Dictionary<string, string> _encrypted;
        private Dictionary<string, string> _encryptedValues;

        public EncryptedDictionary(IEncryptor encryptor)
        {
            Args.ThrowIfNull(encryptor);
            this.Encryptor = encryptor;
            this._unencrypted = new Dictionary<string, string>();
            this._encrypted = new Dictionary<string, string>();
            this._encryptedValues = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            foreach(string key in this._encrypted.Keys)
            {
                output.AppendLine($"{key}.{this._encrypted[key]}");
            }
            return output.ToString();
        }

        public string this[string key] 
        {
            get
            {
                if(_encryptedValues.ContainsKey(key))
                {
                    return _encryptedValues[key];
                }
                else if (_encrypted.ContainsKey(key))
                {
                    return _encrypted[key];
                }
                else if (_unencrypted.ContainsKey(key))
                {
                    return _unencrypted[key];
                }
                
                return null;
            }

            set
            {
                if (_unencrypted.ContainsKey(key))
                {
                    _unencrypted[key] = value;
                }
                else
                {
                    _unencrypted.Add(key, value);
                }

                string encryptedKey = this.Encryptor.EncryptString(key);
                string encryptedValue = this.Encryptor.EncryptString(value);
                if (!_encrypted.ContainsKey(encryptedKey))
                {
                    _encrypted.Add(encryptedKey, encryptedValue);
                }
                else
                {
                    _encrypted[encryptedKey] = encryptedValue;
                }

                if (!_encryptedValues.ContainsKey(key))
                {
                    _encryptedValues.Add(key, encryptedValue);
                }
                else
                {
                    _encryptedValues[key] = encryptedValue;
                }
            }
        }

        public IEncryptor Encryptor
        {
            get;
            private set;
        }

        public ICollection<string> Keys => _encrypted.Keys;

        public ICollection<string> Values => _encrypted.Values;

        public int Count => _encrypted.Count;

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
            this._unencrypted.Clear();
            this._encrypted.Clear();
            this._encryptedValues.Clear();
        }

        public bool Contains(System.Collections.Generic.KeyValuePair<string, string> item)
        {
            if (this._unencrypted.ContainsKey(item.Key))
            {
                return this._unencrypted[item.Key] == item.Value;
            }

            if (this._encrypted.ContainsKey(item.Key))
            {
                return this._encrypted[item.Key] == item.Value;
            }

            return false;
        }

        public bool ContainsKey(string key)
        {
            return this._unencrypted.ContainsKey(key) || this._encrypted.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            bool result = false;
            if (this._unencrypted.ContainsKey(key))
            {
                result = this._unencrypted.Remove(key);
            }
            if (this._encrypted.ContainsKey(key))
            {
                result = this._encrypted.Remove(key);
            }
            if (this._encryptedValues.ContainsKey(key))
            {
                result = this._encryptedValues.Remove(key);
            }

            return result;
        }
    }
}
