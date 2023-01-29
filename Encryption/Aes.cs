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
    public static class Aes
    {
        /// <summary>
        /// Gets a Base64 encoded value representing the cypher of the specified value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Encrypt(string value)
        {
            return Encrypt(value, AesKeyVectorPair.SystemKey);
        }

        /// <summary>
        /// Gets a Base64 encoded value representing the cypher of the specified
        /// value using the specified key.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string value, AesKeyVectorPair key)
        {
            return Encrypt(value, key.Key, key.IV);
        }

        /// <summary>
        /// Encrypts the specified value.
        /// </summary>
        /// <param name="plainText">The value.</param>
        /// <param name="base64EncodedKey">The base64 encoded key.</param>
        /// <param name="base64EncodedIV">The base64 encoded iv.</param>
        /// <returns>Base64 encoded encrypted value</returns>
        public static string Encrypt(string plainText, string base64EncodedKey, string base64EncodedIV)
        {
            AesManaged aes = new AesManaged
            {
                IV = Convert.FromBase64String(base64EncodedIV),
                Key = Convert.FromBase64String(base64EncodedKey)
            };

            ICryptoTransform encryptor = aes.CreateEncryptor();

            byte[] encryptedBytes = Encrypt(plainText, encryptor);
            return Convert.ToBase64String(encryptedBytes);
        }

        public static byte[] Encrypt(string plainText, ICryptoTransform encryptor)
        {
            using (MemoryStream encryptBuffer = new MemoryStream())
            {
                using (CryptoStream encryptStream = new CryptoStream(encryptBuffer, encryptor, CryptoStreamMode.Write))
                {
                    byte[] data = Encoding.UTF8.GetBytes(plainText);
                    encryptStream.Write(data, 0, data.Length);
                    encryptStream.FlushFinalBlock();
                    return encryptBuffer.ToArray();
                }
            }
        }

        public static byte[] EncryptBytes(byte[] plainData, string base64EncodedKey, string base64EncodedIV)
        {
            AesManaged aes = new AesManaged
            {
                IV = Convert.FromBase64String(base64EncodedIV),
                Key = Convert.FromBase64String(base64EncodedKey)
            };

            ICryptoTransform encryptor = aes.CreateEncryptor();

            return EncryptBytes(plainData, encryptor);            
        }

        public static byte[] EncryptBytes(byte[] plainData, ICryptoTransform encryptor)
        {
            using (MemoryStream encryptBuffer = new MemoryStream())
            {
                using (CryptoStream encryptStream = new CryptoStream(encryptBuffer, encryptor, CryptoStreamMode.Write))
                {
                    encryptStream.Write(plainData, 0, plainData.Length);
                    encryptStream.FlushFinalBlock();
                    return encryptBuffer.ToArray();
                }
            }
        }

        /// <summary>
        /// Decrypts the specified base64 encoded value.
        /// </summary>
        /// <param name="base64EncodedValue">The base64 encoded value.</param>
        /// <returns></returns>
        public static string Decrypt(string base64EncodedValue)
        {
            return Decrypt(base64EncodedValue, AesKeyVectorPair.SystemKey);
        }

        /// <summary>
        /// Decrypts the specified base64 encoded value.
        /// </summary>
        /// <param name="base64EncodedValue">The base64 encoded value.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Decrypt(string base64EncodedValue, AesKeyVectorPair key)
        {
            return Decrypt(base64EncodedValue, key.Key, key.IV);
        }

        /// <summary>
        /// Decrypts the specified base64 encoded value.
        /// </summary>
        /// <param name="base64EndoedCipher">The base64 encoded value.</param>
        /// <param name="base64EncodedKey">The base64 encoded key.</param>
        /// <param name="base64EncodedIV">The base64 encoded iv.</param>
        /// <returns></returns>
        public static string Decrypt(string base64EndoedCipher, string base64EncodedKey, string base64EncodedIV, Encoding encoding = null)
        {
            byte[] encData = Convert.FromBase64String(base64EndoedCipher);
            byte[] retBytes = DecryptBytes(encData, base64EncodedKey, base64EncodedIV);
            return (encoding ?? Encoding.UTF8).GetString(retBytes.ToArray());
        }

        public static byte[] DecryptBytes(byte[] encData, string base64EncodedKey, string base64EncodedIV)
        {
            AesManaged aes = new AesManaged
            {
                IV = Convert.FromBase64String(base64EncodedIV),
                Key = Convert.FromBase64String(base64EncodedKey)
            };

            ICryptoTransform decryptor = aes.CreateDecryptor();

            using (MemoryStream decryptBuffer = new MemoryStream(encData))
            {
                using (CryptoStream decryptStream = new CryptoStream(decryptBuffer, decryptor, CryptoStreamMode.Read))
                {
                    byte[] decrypted = new byte[encData.Length];

                    int totalBytesRead = 0;
                    int bytesRead = 0;
                    do
                    {
                        bytesRead = decryptStream.Read(decrypted, totalBytesRead, 1);
                        totalBytesRead += bytesRead;
                    } while (bytesRead > 0);                    

                    // This seems like a cheesy way to remove trailing 0 bytes
                    // but unless I know the expected length of the decrypted data
                    // I can't think of another way to do this effectively
                    List<byte> retBytes = new List<byte>();
                    foreach (byte b in decrypted)
                    {
                        if (b == 0)
                            break;

                        retBytes.Add(b);
                    }

                    return retBytes.ToArray();
                }
            }
        }

        /// <summary>
        /// Encrypts the specified target after converting to xml writing it to the specified 
        /// file path.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static AesKeyVectorPair Encrypt(this object target, string filePath)
        {
            return Encrypt(target, filePath, filePath + ".key", true);
        }

        /// <summary>
        /// Encrypts the specified target using the specified key file after converting to xml, then writes it to the specified 
        /// file path.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="keyFilePath">The key file path.</param>
        /// <returns></returns>
        public static AesKeyVectorPair Encrypt(this object target, string filePath, string keyFilePath)
        {
            return Encrypt(target, filePath, keyFilePath, true);
        }

        public static AesKeyVectorPair Encrypt(this object target, string filePath, string keyFilePath, bool writeKeyFile)
        {
            string text = ToBase64EncodedEncryptedXml(target, out AesKeyVectorPair key);
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(text);
            }

            if (writeKeyFile)
            {
                key.Save(keyFilePath);
            }
            return key;
        }

        public static T Decrypt<T>(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            string keyFile = info.FullName + ".key";
            return Decrypt<T>(filePath, keyFile);
        }

        public static T Decrypt<T>(string filePath, string keyFile)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("The file specified to deserialize from {0} does not exist", filePath));
            }

            if (!File.Exists(keyFile))
            {
                throw new FileNotFoundException(string.Format("The key file specified {0} does not exist", keyFile));
            }

            AesKeyVectorPair key = AesKeyVectorPair.Load(keyFile); 

            return Decrypt<T>(filePath, key);
        }

        public static T Decrypt<T>(string filePath, AesKeyVectorPair key)
        {
            string text;
            using (StreamReader sr = new StreamReader(filePath))
            {
                text = sr.ReadToEnd();                
            }
            return Deserialize<T>(text, key);
        }
        
        /// <summary>
        /// Get a base64 encoded encrypted xml serialization string representing the specified target object
        /// </summary>
        /// <param name="target">The object to serialize</param>
        /// <param name="key">The key used to encrypt and decrypt the resulting string</param>
        /// <returns>string</returns>
        public static string ToBase64EncodedEncryptedXml(this object target, out AesKeyVectorPair key)
        {
            string xml = SerializationExtensions.ToXml(target);
            AesManaged rm = new AesManaged();
            rm.GenerateIV();
            rm.GenerateKey();
            key = new AesKeyVectorPair
            {
                Key = Convert.ToBase64String(rm.Key),
                IV = Convert.ToBase64String(rm.IV)
            };
            byte[] encryptedBytes = Encrypt(xml, rm.CreateEncryptor());
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Deserializes the specified base64 encrypted XML string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="base64EncryptedXmlString">The base64 encrypted XML string.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string base64EncryptedXmlString, AesKeyVectorPair key)
        {
            string xml = Decrypt(base64EncryptedXmlString, key.Key, key.IV);
            return SerializationExtensions.FromXml<T>(xml);
        }
    }
}
