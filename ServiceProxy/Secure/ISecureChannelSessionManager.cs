using Bam.Net.Data.Repositories;
using Bam.Net.Server.ServiceProxy.Data;
using Bam.Net.Server.ServiceProxy.Data.Dao.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Secure
{
    public interface ISecureChannelSessionManager
    {

        SecureChannelSession GetSecureChannelSessionForContext(IHttpContext httpContext, Instant clientNow = null);

        string GetSecureChannelSessionIdentifier(IRequest request);

        SecureChannelSession CreateSecureChannelSession(IResponse response, ServiceProxyServerRepository repository, Instant clientNow);

        SecureChannelSession RetrieveSecureChannelSession(string sessionIdentifier, ServiceProxyServerRepository repository);
    }
}
