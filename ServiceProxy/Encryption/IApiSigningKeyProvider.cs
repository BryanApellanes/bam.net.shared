/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
namespace Bam.Net.ServiceProxy.Encryption
{
    public interface IApiSigningKeyProvider
    {
        ApiSigningKeyInfo GetApiSigningKeyInfo(IApplicationNameProvider nameProvider);
        string GetApplicationApiKey(string applicationClientId, int index);
        string GetApplicationClientId(IApplicationNameProvider nameProvider);
        string GetCurrentApiKey();
    }
}
