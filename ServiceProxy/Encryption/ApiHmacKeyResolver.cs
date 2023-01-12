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
            ApiHmacKeyProvider = DefaultConfigurationApiKeyProvider.Instance;
            ApplicationNameProvider = DefaultConfigurationApplicationNameProvider.Instance;
            HashAlgorithm = HashAlgorithms.SHA256;
        }

        public ApiHmacKeyResolver(IApiHmacKeyProvider apiKeyProvider)
            : this()
        {
            ApiHmacKeyProvider = apiKeyProvider;
        }

        public ApiHmacKeyResolver(IApplicationNameProvider nameProvider)
            : this()
        {
            ApplicationNameProvider = nameProvider;
        }

        public ApiHmacKeyResolver(IApiHmacKeyProvider apiKeyProvider, IApplicationNameProvider nameProvider) : this()
        {
            ApiHmacKeyProvider = apiKeyProvider;
            ApplicationNameProvider = nameProvider;
        }

        public static ApiHmacKeyResolver Default
        {
            get;
        }

        public IApiArgumentEncoder ApiArgumentEncoder { get; set; }

        public IApiHmacKeyProvider ApiHmacKeyProvider
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
            return ApiHmacKeyProvider.GetApiHmacKeyInfo(nameProvider);
        }

        public string GetApplicationApiHmacKey(string applicationClientId, int index)
        {
            return ApiHmacKeyProvider.GetApplicationApiHmacKey(applicationClientId, index);
        }

        public string GetApplicationClientId(IApplicationNameProvider nameProvider)
        {
            return ApiHmacKeyProvider.GetApplicationClientId(nameProvider);
        }

        public string GetCurrentApiHmacKey()
        {
            return ApiHmacKeyProvider.GetCurrentApiHmacKey();
        }

        #endregion

        #region IApplicationNameResolver Members

        public string GetApplicationName()
        {
            return ApplicationNameProvider.GetApplicationName();
        }

        #endregion
        

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
                result = IsValidHmac(stringToHash, token);
            }

            return result;
        }
        
        public bool IsValidHmac(string stringToHash, string hmac)
        {
            string checkHmac = GetHmac(stringToHash);
            return hmac.Equals(checkHmac);
        }
    }
}
