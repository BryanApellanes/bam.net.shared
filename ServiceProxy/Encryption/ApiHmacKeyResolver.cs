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

namespace Bam.Net.ServiceProxy.Encryption
{
    /// <summary>
    /// A class used to provide the functionality
    /// of both an ApiSigningKeyProvider and an ApplicationNameProvider
    /// </summary>
    public partial class ApiHmacKeyResolver : IApiHmacKeyProvider, IApplicationNameProvider, IApiHmacKeyResolver
    {
        static ApiHmacKeyResolver()
        {
            Default = new ApiHmacKeyResolver();
        }

        public ApiHmacKeyResolver()
        {
            ApiSigningKeyProvider = DefaultConfigurationApiKeyProvider.Instance;
            ApplicationNameProvider = DefaultConfigurationApplicationNameProvider.Instance;
            HashAlgorithm = HashAlgorithms.SHA256;
        }

        public ApiHmacKeyResolver(IApiHmacKeyProvider apiKeyProvider)
            : this()
        {
            ApiSigningKeyProvider = apiKeyProvider;
        }

        public ApiHmacKeyResolver(IApplicationNameProvider nameProvider)
            : this()
        {
            ApplicationNameProvider = nameProvider;
        }

        public ApiHmacKeyResolver(IApiHmacKeyProvider apiKeyProvider, IApplicationNameProvider nameProvider) : this()
        {
            ApiSigningKeyProvider = apiKeyProvider;
            ApplicationNameProvider = nameProvider;
        }

        public static ApiHmacKeyResolver Default
        {
            get;
        }

        public IApiArgumentEncoder ApiArgumentEncoder { get; set; }

        public IApiHmacKeyProvider ApiSigningKeyProvider
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

        public ApiHmacKeyInfo GetApiHmacKeyInfo(IApplicationNameProvider nameProvider)
        {
            return ApiSigningKeyProvider.GetApiHmacKeyInfo(nameProvider);
        }

        public string GetApplicationApiHmacKey(string applicationClientId, int index)
        {
            return ApiSigningKeyProvider.GetApplicationApiHmacKey(applicationClientId, index);
        }

        public string GetApplicationClientId(IApplicationNameProvider nameProvider)
        {
            return ApiSigningKeyProvider.GetApplicationClientId(nameProvider);
        }

        public string GetCurrentApiHmacKey()
        {
            return ApiSigningKeyProvider.GetCurrentApiHmacKey();
        }

        #endregion

        #region IApplicationNameResolver Members

        public string GetApplicationName()
        {
            return ApplicationNameProvider.GetApplicationName();
        }

        #endregion
        
/*        public void SetHmacHeader(HttpRequestMessage request, string stringToHash)
        {
            request.Headers.Add(Headers.Hmac, GetHmac(stringToHash));
        }

        public void SetHmacHeader(HttpWebRequest request, string stringToHash)
        {
            SetHmacHeader(request.Headers, stringToHash);
        }
       
        public void SetHmacHeader(NameValueCollection headers, string stringToHash)
        {
            headers[Headers.Hmac] = GetHmac(stringToHash);
        }*/

        public string GetHmac(string stringToHash)
        {
            ApiHmacKeyInfo apiKey = this.GetApiHmacKeyInfo(this);
            return stringToHash.HmacHexString(apiKey.ApiHmacKey, HashAlgorithm);
        }

        // TODO: fix this to use ServiceProxyInvocationRequest
        public bool IsValidRequest(ServiceProxyInvocation request)
        {
            Args.ThrowIfNull(request, "request");
			
            string className = request.ClassName;
            string methodName = request.MethodName;
            string stringToHash = ApiArgumentEncoder.GetValidationString(className, methodName, "");//, request.ArgumentsAsJsonArrayOfJsonStrings);

            string token = request.Context.Request.Headers[Headers.Hmac];
            bool result = false;
            if (!string.IsNullOrEmpty(token))
            {
                result = IsValidKeyToken(stringToHash, token);
            }

            return result;
        }
        
        public bool IsValidKeyToken(string stringToHash, string token)
        {
            string checkToken = GetHmac(stringToHash);
            return token.Equals(checkToken);
        }
    }
}
