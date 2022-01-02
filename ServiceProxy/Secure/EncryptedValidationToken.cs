/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net.ServiceProxy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Secure
{
    public class EncryptedValidationToken
    {
        public EncryptedValidationToken()
        {
            Algorithm = HashAlgorithms.SHA256;
        }

        public string NonceCipher
        {
            get;
            set;
        }

        public string HashCipher
        {
            get;
            set;
        }

        public HashAlgorithms Algorithm { get; set; }

        public ValidationToken Decrypt(SecureChannelSession secureChannelSession, bool usePkcsPadding = false)
        {
            return new ValidationToken
            {
                Nonce = secureChannelSession.DecryptWithPrivateKey(NonceCipher, usePkcsPadding),
                Hash = secureChannelSession.DecryptWithPrivateKey(HashCipher, usePkcsPadding),
                Algorithm = Algorithm,
            };
        }
    }
}
