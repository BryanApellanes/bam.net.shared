using Bam.Net.Data.Repositories;
using Bam.Net.ServiceProxy.Data.Dao.Repository;
using Bam.Net.ServiceProxy.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Encryption
{
    public interface ISecureChannelSessionDataManager 
    {
        int SessionExpirationMinutes { get; set; }

        ServiceProxyDataRepository ServiceProxyDataRepository { get; set; }

        Task SetSessionKeyAsync(IHttpContext httpContext, SetSessionKeyRequest setSessionKeyRequest);

        Task<SecureChannelSession> GetSecureChannelSessionForContextAsync(IHttpContext httpContext, Instant clientNow = null);

        string GetSecureChannelSessionIdentifier(IRequest request);

        string GetSecureChannelSessionClientDescriptor(IRequest request);

        Task<SecureChannelSession> CreateSecureChannelSessionAsync(Instant clientNow);

        Task<SecureChannelSession> StartSecureChannelSessionAsync(IResponse response, Instant clientNow);

        Task<SecureChannelSession> StartSecureChannelSessionAsync(IResponse response, ServiceProxyDataRepository repository, Instant clientNow);
        
        Task<SecureChannelSession> RetrieveSecureChannelSessionAsync(string sessionIdentifier);

        Task<SecureChannelSession> RetrieveSecureChannelSessionAsync(string sessionIdentifier, ServiceProxyDataRepository repository);

        Task EndSecureChannelSessionAsync(string sessionIdentifier);

        Task EndSecureChannelSessionAsync(string sessionIdentifier, ServiceProxyDataRepository repository);
    }
}
