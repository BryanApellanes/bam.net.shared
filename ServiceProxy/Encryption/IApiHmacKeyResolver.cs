using Bam.Net.Server.ServiceProxy;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;

namespace Bam.Net.ServiceProxy.Encryption
{
    public interface IApiHmacKeyResolver
    {
        HashAlgorithms HashAlgorithm { get; set; }
        /// <summary>
        /// When implemented by a derived class hashes the specified
        /// stringToHash using the key/shared secret
        /// </summary>
        /// <param name="stringToHash"></param>
        /// <returns></returns>
        string GetHmac(string stringToHash);
        ApiHmacKeyInfo GetApiHmacKeyInfo(IApplicationNameProvider nameProvider);
        string GetApplicationApiHmacKey(string applicationClientId, int index);
        string GetCurrentApiHmacKey();
        bool IsValidHmac(string stringToValidate, string hmac);
    }
}