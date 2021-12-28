using Bam.Net.Caching;
using Bam.Net.Data;
using Bam.Net.Data.Repositories;
using Bam.Net.Server.ServiceProxy.Data;
using Bam.Net.Server.ServiceProxy.Data.Dao.Repository;
using Bam.Net.Services;
using Bam.Net.Web;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Bam.Net.ServiceProxy.Secure
{
    public class SecureChannelSessionManager : ISecureChannelSessionManager
    {
        public SecureChannelSessionManager()
        {
            this.ServiceProxyServerRepository = new ServiceProxyServerRepository();
            this.CachingRepository = new CachingRepository(this.ServiceProxyServerRepository);
        }

        [Inject]
        public CachingRepository CachingRepository { get; set; }

        [Inject]
        public ServiceProxyServerRepository ServiceProxyServerRepository { get; set; }

        public virtual SecureChannelSession GetSecureChannelSessionForContext(IHttpContext httpContext, Instant clientNow = null)
        {
            string existingSecureChannelSessionId = GetSecureChannelSessionIdentifier(httpContext.Request);
            SecureChannelSession secureChannelSession;
            if (string.IsNullOrEmpty(existingSecureChannelSessionId))
            {
                secureChannelSession = CreateSecureChannelSession(httpContext.Response, ServiceProxyServerRepository, clientNow ?? new Instant());
            }
            else
            {
                secureChannelSession = RetrieveSecureChannelSession(existingSecureChannelSessionId, ServiceProxyServerRepository);
            }

            return secureChannelSession;
        }

        public SecureChannelSession CreateSecureChannelSession(IResponse response, ServiceProxyServerRepository repository, Instant clientNow)
        {
            SecureChannelSession secureChannelSession = new SecureChannelSession(clientNow, true).Initialize();
            Cookie secureChannelSessionIdCookie = new Cookie(SecureChannelSession.CookieName, secureChannelSession.Identifier);
            response.Cookies.Add(secureChannelSessionIdCookie);
            secureChannelSession = ServiceProxyServerRepository.Save(secureChannelSession);

            return secureChannelSession;
        }

        public string GetSecureChannelSessionIdentifier(IRequest request)
        {
            string secureChannelSessionId = GetSecureChannelSessionIdFromCookie(request);
            if (string.IsNullOrEmpty(secureChannelSessionId))
            {
                secureChannelSessionId = GetSecureChannelSessionIdFromHeader(request);
            }
            return secureChannelSessionId;
        }

        public SecureChannelSession RetrieveSecureChannelSession(string sessionIdentifier, ServiceProxyServerRepository repository)
        {
            return repository.Query<SecureChannelSession>(secureChannelSession => secureChannelSession.Identifier == sessionIdentifier).FirstOrDefault();
        }

        public void EndSecureChannelSession(string sessionIdentifier, ServiceProxyServerRepository repository)
        {
            SecureChannelSession secureChannelSession = repository.Query<SecureChannelSession>(scs => scs.Identifier == sessionIdentifier).FirstOrDefault();
            secureChannelSession.SymmetricKey = null;
            secureChannelSession.SymmetricIV = null;
            secureChannelSession.AsymmetricKey = null;
            secureChannelSession.Deleted = DateTime.UtcNow;
            repository.Save(secureChannelSession);
        }

        protected string GetSecureChannelSessionIdFromCookie(IRequest request)
        {
            Cookie secureChannelSessionCookie = request.Cookies[SecureChannelSession.CookieName];
            if(secureChannelSessionCookie != null)
            {
                return secureChannelSessionCookie.Value;
            }
            return null;
        }

        protected string GetSecureChannelSessionIdFromHeader(IRequest request)
        {
            return request.Headers[Headers.SecureChannelSessionId];            
        }

        public void SetSessionKey(IHttpContext httpContext, SetSessionKeyRequest setSessionKeyRequest)
        {
            SecureChannelSession secureChannelSession = GetSecureChannelSessionForContext(httpContext);
            secureChannelSession.SetSymmetricKey(setSessionKeyRequest);
            ServiceProxyServerRepository.Save(secureChannelSession);
        }
    }
}
