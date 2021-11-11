/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;

namespace Bam.Net.Encryption
{
    public class Encrypted
    {
        protected static readonly string DefaultIV = Convert.ToBase64String(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

        public Encrypted()
        {
            SecureRandom random = new SecureRandom();
            this.Key = random.GenerateSeed(16);
            this.IV = random.GenerateSeed(16);
            this.Plain = string.Empty;
        }

        public Encrypted(string data)
            : this()
        {
            this.Plain = data;
        }

        public Encrypted(string data, string b64Key)
            : this(data, b64Key, DefaultIV)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">The plain text data to be encrypted</param>
        /// <param name="b64Key">A base 64 encoded key</param>
        /// <param name="b64IV">A base 64 encoded initialization vector</param>
        public Encrypted(string data, string b64Key, string b64IV)
        {
            this.Plain = data;

            this.Base64Key = b64Key;
            this.Base64IV = b64IV;

            this.Cipher = Encrypt();
        }

        protected Encrypted(byte[] cipher, byte[] key, byte[] iv)
        {
            this.Key = key;
            this.Cipher = cipher;
            this.IV = iv;
        }

        public static implicit operator string(Encrypted enc)
        {
            return enc.Value;
        }

        public virtual string Value => Base64Cipher;

        public byte[] Key
        {
            get;
            set;
        }

        public byte[] IV
        {
            get;
            private set;
        }

        public string Salt => ";".RandomLetters(1); // TODO: review this for validity - why 1, - are things dependent on this? 

        public string Plain
        {
            get;
            set;
        }

        public byte[] Cipher
        {
            get;
            private set;
        }

        public string Base64Cipher
        {
            get
            {
                if (Cipher == null)
                {
                    Encrypt();
                }

                return Convert.ToBase64String(Cipher);
            }
            protected set => Cipher = Convert.FromBase64String(value);
        }

        /// <summary>
        /// Gets the base64 encoded key.
        /// </summary>
        protected string Base64Key
        {
            get => Key != null ? Convert.ToBase64String(Key) : string.Empty;
            set => Key = Convert.FromBase64String(value);
        }

        /// <summary>
        /// Gets the base64 encoded initialization vector.
        /// </summary>
        protected string Base64IV
        {
            get => IV != null ? Convert.ToBase64String(IV) : string.Empty;
            set
            {
                IV = Convert.FromBase64String(value);
            }
        }

        private byte[] Encrypt()
        {
            Base64Cipher = Aes.Encrypt(string.Concat(Plain, Salt), Base64Key, Base64IV);
            return Cipher;
        }
    }
}
