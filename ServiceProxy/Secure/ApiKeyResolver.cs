/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net;
using Bam.Net.Web;
using Bam.Net.Configuration;
using System.Net.Http;
using Bam.Net.Server.ServiceProxy;

namespace Bam.Net.ServiceProxy.Secure
{
    /// <summary>
    /// A class used to provide the functionality
    /// of both an ApiKeyProvider and an ApplicationNameProvider
    /// </summary>
    public partial class ApiKeyResolver : IApiKeyProvider, IApplicationNameProvider, IApiKeyResolver
    {
        static ApiKeyResolver()
        {
            Default = new ApiKeyResolver();
        }

        public ApiKeyResolver()
        {
            ApiKeyProvider = DefaultConfigurationApiKeyProvider.Instance;
            ApplicationNameProvider = DefaultConfigurationApplicationNameProvider.Instance;
            HashAlgorithm = HashAlgorithms.SHA256;
        }

        public ApiKeyResolver(IApiKeyProvider apiKeyProvider)
            : this()
        {
            ApiKeyProvider = apiKeyProvider;
        }

        public ApiKeyResolver(IApplicationNameProvider nameProvider)
            : this()
        {
            ApplicationNameProvider = nameProvider;
        }

        public ApiKeyResolver(IApiKeyProvider apiKeyProvider, IApplicationNameProvider nameProvider) : this()
        {
            ApiKeyProvider = apiKeyProvider;
            ApplicationNameProvider = nameProvider;
        }

        public static ApiKeyResolver Default
        {
            get;
        }

        public IApiArgumentEncoder ApiArgumentEncoder { get; set; }

        public IApiKeyProvider ApiKeyProvider
        {
            get;
            set;
        }

        public IApplicationNameProvider ApplicationNameProvider
        {
            get;
            set;
        }

        public HashAlgorithms HashAlgorithm { get; set; }

        #region IApiKeyProvider Members

        public ApiKeyInfo GetApiKeyInfo(IApplicationNameProvider nameProvider)
        {
            return ApiKeyProvider.GetApiKeyInfo(nameProvider);
        }

        public string GetApplicationApiKey(string applicationClientId, int index)
        {
            return ApiKeyProvider.GetApplicationApiKey(applicationClientId, index);
        }

        public string GetApplicationClientId(IApplicationNameProvider nameProvider)
        {
            return ApiKeyProvider.GetApplicationClientId(nameProvider);
        }

        public string GetCurrentApiKey()
        {
            return ApiKeyProvider.GetCurrentApiKey();
        }

        #endregion

        #region IApplicationNameResolver Members

        public string GetApplicationName()
        {
            return ApplicationNameProvider.GetApplicationName();
        }

        #endregion
        
        public void SetKeyToken(HttpRequestMessage request, string stringToHash)
        {
            request.Headers.Add(Headers.KeyToken, CreateKeyToken(stringToHash));
        }

        public void SetKeyToken(HttpWebRequest request, string stringToHash)
        {
            SetKeyToken(request.Headers, stringToHash);
        }
       
        public void SetKeyToken(NameValueCollection headers, string stringToHash)
        {
            headers[Headers.KeyToken] = CreateKeyToken(stringToHash);
        }

        public string CreateKeyToken(string stringToHash)
        {
            ApiKeyInfo apiKey = this.GetApiKeyInfo(this);
            return $"{apiKey.ApiKey}:{stringToHash}".HmacHexString(apiKey.ApiKey, HashAlgorithm);
        }

        // TODO: fix this to use ServiceProxyInvocationRequest
        public bool IsValidRequest(ServiceProxyInvocation request)
        {
            Args.ThrowIfNull(request, "request");
			
            string className = request.ClassName;
            string methodName = request.MethodName;
            string stringToHash = ApiArgumentEncoder.GetStringToHash(className, methodName, "");//, request.ArgumentsAsJsonArrayOfJsonStrings);

            string token = request.Context.Request.Headers[Headers.KeyToken];
            bool result = false;
            if (!string.IsNullOrEmpty(token))
            {
                result = IsValidKeyToken(stringToHash, token);
            }

            return result;
        }
        
        public bool IsValidKeyToken(string stringToHash, string token)
        {
            string checkToken = CreateKeyToken(stringToHash);
            return token.Equals(checkToken);
        }
    }
}
