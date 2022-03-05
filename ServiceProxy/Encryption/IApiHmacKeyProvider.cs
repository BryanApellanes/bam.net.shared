/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
namespace Bam.Net.ServiceProxy.Encryption
{
    public interface IApiHmacKeyProvider
    {
        ApiHmacKeyInfo GetApiHmacKeyInfo(IApplicationNameProvider nameProvider);
        string GetApplicationApiSigningKey(string applicationClientId, int index);
        string GetApplicationClientId(IApplicationNameProvider nameProvider);
        string GetCurrentApiKey();
    }
}
