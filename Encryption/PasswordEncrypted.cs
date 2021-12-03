/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace Bam.Net.Encryption
{
    /// <summary>
    /// Represents a class that encrypts data with a provided password.
    /// </summary>
    public class PasswordEncrypted
    {
        protected PasswordEncrypted() { }

        /// <summary>
        /// Create a PasswordEncrypted instance.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="password">The password used to encrypt the data.</param>
        public PasswordEncrypted(string data, string password)
        {
            this.Data = data;
            this.Encrypt(password);
        }

        public static implicit operator string(PasswordEncrypted p)
        {
            return p.Cipher;
        }

        public virtual string Value => Cipher;

        public string Data
        {
            get;
            protected set;
        }

        public string Cipher
        {
            get;
            internal set;
        }

        string _password;
        protected internal string Password
        {
            get => _password;
            set => Encrypt(value);
        }

        public string Encrypt(string password)
        {
            _password = password;
            Cipher = Rijndael.EncryptStringAES(Data, password);
            return Cipher;
        }
    }

}
