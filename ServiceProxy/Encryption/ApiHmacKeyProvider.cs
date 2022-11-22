/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Configuration;
using Bam.Net.ServiceProxy;

namespace Bam.Net.ServiceProxy.Encryption
{
    /// <summary>
    /// A class used to retrieve an applications Api Hmac Key and 
    /// client Id used in EncryptedServiceProxy sessions.
    /// Implementers of this class need only implement the
    /// GetApplicationClientId and GetApplicationApiKey methods, 
    /// retrieving each from an appropriate location.  For example,
    /// the DefaultConfigurationApiKeyProvider retrieves this
    /// information from the web.config or app.config file.
    /// </summary>
    public abstract class ApiHmacKeyProvider : IApiHmacKeyProvider
    {
        public ApiHmacKeyInfo GetApiHmacKeyInfo(IApplicationNameProvider nameProvider)
        {
            string clientId = GetApplicationClientId(nameProvider);
            ApiHmacKeyInfo info = new ApiHmacKeyInfo()
            {
                ApiHmacKey = GetApplicationApiHmacKey(clientId, 0),
                ApplicationClientId = clientId
            };
            return info;
        }

        public string GetCurrentApiHmacKey()
        {
            return GetApplicationApiHmacKey(GetApplicationClientId(ApplicationNameProvider.Default), 0);
        }

        public abstract string GetApplicationClientId(IApplicationNameProvider nameProvider);

        public abstract string GetApplicationApiHmacKey(string applicationClientId, int index);

    }
}
