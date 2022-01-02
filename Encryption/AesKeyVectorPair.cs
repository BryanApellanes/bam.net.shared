/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Bam.Net.Configuration;

namespace Bam.Net.Encryption
{
    [Serializable]
    public class AesKeyVectorPair
    {
        public const string SystemKeyFileName = "bamkey.aes";

        public AesKeyVectorPair()
        {
            SetKeyAndIv();
        }

        public AesKeyVectorPair(string base64EncodedKey, string base64EncdoedIV)
        {
            this.Key = base64EncodedKey;
            this.IV = base64EncdoedIV;
        }

        static readonly object _aesLock = new object();
        static volatile AesKeyVectorPair _key;

        /// <summary>
        /// Gets the advanced encryption key vector pair for the currently running bam system.
        /// </summary>
        public static AesKeyVectorPair SystemKey
        {
            get
            {
                if (_key == null)
                {
                    lock(_aesLock)
                    {
                        string fileName = Path.Combine(BamHome.Local, SystemKeyFileName);
                        if (File.Exists(fileName))
                        {
                            _key = Load(fileName);
                        }
                        else
                        {
                            _key = new AesKeyVectorPair();
                            _key.Save(fileName);
                        }
                    }
                }

                return _key;
            }
        }

        private void SetKeyAndIv()
        {
            AesManaged aes = new AesManaged();
            aes.GenerateKey();
            aes.GenerateIV();
            this.Key = Convert.ToBase64String(aes.Key);
            this.IV = Convert.ToBase64String(aes.IV);
        }

        public void Save(string filePath)
        {
            string xml = SerializationExtensions.ToXml(this);
            byte[] xmlBytes = Encoding.UTF8.GetBytes(xml);
            string xmlBase64 = Convert.ToBase64String(xmlBytes);
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(xmlBase64);
            }
        }

        public static AesKeyVectorPair Load(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string xmlBase64 = sr.ReadToEnd();
                byte[] xmlBytes = Convert.FromBase64String(xmlBase64);
                string xml = Encoding.UTF8.GetString(xmlBytes);
                return SerializationExtensions.FromXml<AesKeyVectorPair>(xml);
            }
        }

        /// <summary>
        /// Gets or sets the base 64 encoded key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the base 64 encoded initialization vector.
        /// </summary>
        /// <value>
        /// The iv.
        /// </value>
        public string IV { get; set; }

        /// <summary>
        /// Gets a Base64 encoded value representing the cypher of the specified
        /// value using the current key.
        /// </summary>
        public string Encrypt(string value)
        {
            return Aes.Encrypt(value, this);
        }

        /// <summary>
        /// Decrypts the specified base64 encoded value.
        /// </summary>
        /// <param name="base64EncodedCipher">The base64 encoded value.</param>
        /// <returns></returns>
        public string Decrypt(string base64EncodedCipher)
        {
            return Aes.Decrypt(base64EncodedCipher, this);
        }
    }
}
