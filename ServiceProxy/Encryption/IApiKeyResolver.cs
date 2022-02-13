using Bam.Net.Server.ServiceProxy;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;

namespace Bam.Net.ServiceProxy.Encryption
{
    public interface IApiKeyResolver
    {
        HashAlgorithms HashAlgorithm { get; set; }
        /// <summary>
        /// When implemented by a derived class hashes the specified
        /// stringToHash using the key/shared secret
        /// </summary>
        /// <param name="stringToHash"></param>
        /// <returns></returns>
        string CreateKeyToken(string stringToHash);
        ApiKeyInfo GetApiKeyInfo(IApplicationNameProvider nameProvider);
        string GetApplicationApiKey(string applicationClientId, int index);
        string GetApplicationClientId(IApplicationNameProvider nameProvider);
        string GetApplicationName();
        string GetCurrentApiKey();
        bool IsValidRequest(ServiceProxyInvocation request);
        bool IsValidKeyToken(string stringToHash, string token);

        void SetKeyToken(HttpRequestMessage request, string stringToHash);

        [Obsolete("Use SetKeyToken(HttpRequestMessage) instead.")]
        void SetKeyToken(NameValueCollection headers, string stringToHash);

        [Obsolete("Use SetKeyToken(HttpRequestMessage) instead.")]
        void SetKeyToken(HttpWebRequest request, string stringToHash);
    }
}