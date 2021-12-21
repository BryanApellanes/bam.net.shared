/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Encryption;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Engines;

namespace Bam.Net.ServiceProxy.Secure
{
    public class ClientSessionInfo
    {
        /// <summary>
        /// The value of the session cookie.
        /// </summary>
        public string ClientIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the server Rsa public key of the current session as a Pem string.
        /// </summary>
        public string PublicKey
        {
            get;
            set;
        }

        /// <summary>
        /// The key for the current session.
        /// </summary>
        protected internal string SessionKey
        {
            get;
            set;
        }

        /// <summary>
        /// The initialization vector for the current session
        /// </summary>
        protected internal string SessionIV
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the base 64 encoded symetric cipher for the specified plain text.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string GetSymetricCipher(string plainText)
        {
            Encrypted encrypted = new Encrypted(plainText, SessionKey, SessionIV);
            return encrypted.Base64Cipher;
        }

        public byte[] GetSymetricCipherBytes(string plainText)
        {
            Encrypted encrypted = new Encrypted(plainText, SessionKey, SessionIV);
            return encrypted.Cipher;
        }

        public string GetPlainText(byte[] cipherBytes)
        {
            string base64Cipher = Convert.ToBase64String(cipherBytes);
            Decrypted decrypted = new Decrypted(base64Cipher, SessionKey, SessionIV);
            return decrypted.Value;
        }

        public byte[] GetPlainBytes(byte[] cipherBytes, Encoding encoding = null)
        {
            string base64Cipher = Convert.ToBase64String(cipherBytes);
            Decrypted decrypted = new Decrypted(base64Cipher, SessionKey, SessionIV);
            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetBytes(decrypted.Value);
        }

        /// <summary>
        /// Gets a base 64 encoded asymmetric cipher of the specified input.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string GetAsymetricCipher(string plainText, Encoding encoding = null)
        {
            return plainText.EncryptWithPublicKey(PublicKey, encoding);
        }

        public byte[] GetAsymetricCipherBytes(string plainText, Encoding encoding = null)
        {
            return plainText.GetPublicKeyEncryptedBytes(PublicKey.ToKey(), encoding);
        }

        public byte[] GetAsymetricCipherBytes(byte[] plainData)
        {
            return plainData.GetPublicKeyEncryptedBytes(PublicKey.ToKey());
        }

        public override bool Equals(object obj)
        {
            if (obj is ClientSessionInfo info)
            {
                return info.ClientIdentifier.Equals(ClientIdentifier) && info.PublicKey.Equals(PublicKey);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.GetHashCode(ClientIdentifier, PublicKey);
        }

        public override string ToString()
        {
            return $"ClientIdentifier={ClientIdentifier};PublicKey={PublicKey}";
        }
    }
}
