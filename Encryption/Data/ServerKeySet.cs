using Bam.Net.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Bam.Net.Encryption.Data
{
    /// <summary>
    /// Represents a key set for the current process or host to use 
    /// in communication as the server.
    /// </summary>
    public class ServerKeySet : KeySet, IServerKeySet, IRsaKeySource
    {
        public ServerKeySet() : base(RsaKeyLength._2048, true, false)
        {
            this.MachineName = Environment.MachineName;
            this.ServerHostName = Dns.GetHostName();
        }

        [CompositeKey]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine that instantiated this keyset.
        /// </summary>
        [CompositeKey]
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the dns hostname of the machine that instantiated this keyset.
        /// </summary>
        [CompositeKey]
        public string ServerHostName { get; set; }

        [CompositeKey]
        public string ClientHostName { get; set; }

        public bool GetIsAesInitialized()
        {
            return !string.IsNullOrEmpty(AesKey) && !string.IsNullOrEmpty(AesIV);
        }

        public bool GetIsRsaInitialized()
        {
            return !string.IsNullOrEmpty(RsaKey);
        }

        /// <summary>
        /// Gets the rsa key for the current server keyset.
        /// </summary>
        /// <returns></returns>
        public RsaPublicPrivateKeyPair GetRsaKey()
        {
            return new RsaPublicPrivateKeyPair(RsaKey) { RsaKeyLength = RsaKeyLength };
        }

        public RsaPublicKey GetRsaPublicKey()
        {
            return GetRsaKey().GetRsaPublicKey();
        }

        public ISecretExchange GetSecretExchange()
        {
            // Only send if the current machine created this key set.
            if (MachineName.Equals(Environment.MachineName)) 
            {
                if (!GetIsAesInitialized())
                {
                    InitializeAesKey();
                }

                return new SecretExchange
                {
                    Identifier = this.Identifier,
                    ServerHostName = ServerHostName,
                    ClientHostName = ClientHostName,
                    SecretCipher = Encrypt(Secret)
                };
            }

            throw new InvalidOperationException($"The {nameof(ServerKeySet)} with identifier {Identifier} is not from the current machine ({Environment.MachineName})");
        }

        public string PrivateKeyDecrypt(string value)
        {
            return value.DecryptWithPrivateKey(GetAsymmetricKeys());
        }
    }
}
