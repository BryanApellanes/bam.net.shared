using Bam.Net.Caching;
using Bam.Net.Data;
using Bam.Net.Data.Repositories;
using Bam.Net.ServiceProxy.Data.Dao.Repository;
using Bam.Net.ServiceProxy.Data;
using Bam.Net.Services;
using Bam.Net.Web;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Application;
using Bam.Net.Encryption;
using Bam.Net.CoreServices.ApplicationRegistration.Data;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class SecureChannelSessionDataManager : ISecureChannelSessionDataManager
    {
        public SecureChannelSessionDataManager(ApiConf apiConfig = null)
        {
            this.ApiConfig = apiConfig ?? new ApiConf();
            this.ServiceProxyDataRepository = new ServiceProxyDataRepository();
            this.ServerKeySetDataManager = new ServerKeySetDataManager();
        }

        public ApiConf ApiConfig { get; }

        [Inject]
        public ServiceProxyDataRepository ServiceProxyDataRepository { get; set; }

        [Inject]
        public IServerKeySetDataManager ServerKeySetDataManager { get; set; }

        public int SessionExpirationMinutes
        {
            get => ApiConfig.SessionExpirationMinutes;
            set => ApiConfig.SessionExpirationMinutes = value;
        }

        public virtual async Task<SecureChannelSession> GetSecureChannelSessionForContextAsync(IHttpContext httpContext, Instant clientNow = null)
        {
            string existingSecureChannelSessionId = GetSecureChannelSessionIdentifier(httpContext.Request);
            SecureChannelSession secureChannelSession;
            if (string.IsNullOrEmpty(existingSecureChannelSessionId))
            {
                secureChannelSession = await StartSecureChannelSessionAsync(httpContext.Response, ServiceProxyDataRepository, clientNow ?? new Instant());
                secureChannelSession.Server = httpContext?.Request?.Url?.Authority;
                secureChannelSession.Client = GetSecureChannelSessionClientDescriptor(httpContext.Request);
                secureChannelSession = await ServiceProxyDataRepository.SaveAsync(secureChannelSession);
                ProcessDescriptor clientProcess = ProcessDescriptor.Parse(secureChannelSession.Client);
                _ = ServerKeySetDataManager.CreateServerKeySetAsync(clientProcess.MachineName);
            }
            else
            {
                secureChannelSession = await RetrieveSecureChannelSessionAsync(existingSecureChannelSessionId, ServiceProxyDataRepository);
            }

            return secureChannelSession;
        }

        public async Task<SecureChannelSession> CreateSecureChannelSessionAsync(Instant clientNow)
        {
            SecureChannelSession secureChannelSession = new SecureChannelSession(clientNow, true);
            secureChannelSession = await ServiceProxyDataRepository.SaveAsync(secureChannelSession);
            return secureChannelSession;
        }

        public async Task<SecureChannelSession> StartSecureChannelSessionAsync(IResponse response, Instant clientNow)
        {
            return await StartSecureChannelSessionAsync(response, ServiceProxyDataRepository, clientNow);
        }

        public async Task<SecureChannelSession> StartSecureChannelSessionAsync(IResponse response, ServiceProxyDataRepository repository, Instant clientNow)
        {
            SecureChannelSession secureChannelSession = await CreateSecureChannelSessionAsync(clientNow);
            Cookie secureChannelSessionIdCookie = new Cookie(SecureChannelSession.CookieName, secureChannelSession.Identifier);
            response.Cookies.Add(secureChannelSessionIdCookie);

            return secureChannelSession;
        }

        public string GetSecureChannelSessionIdentifier(IRequest request)
        {
            Args.ThrowIfNull(request, nameof(request));

            string secureChannelSessionId = GetSecureChannelSessionIdFromCookie(request);
            if (string.IsNullOrEmpty(secureChannelSessionId))
            {
                secureChannelSessionId = GetSecureChannelSessionIdFromHeader(request);
            }
            return secureChannelSessionId;
        }

        public string GetSecureChannelSessionClientDescriptor(IRequest request)
        {
            Args.ThrowIfNull(request, nameof(request));
            return request.Headers[Headers.ProcessDescriptor];
        }

        public async Task<SecureChannelSession> RetrieveSecureChannelSessionAsync(string sessionIdentifier)
        {
            return await RetrieveSecureChannelSessionAsync(sessionIdentifier, ServiceProxyDataRepository);
        }

        public Task<SecureChannelSession> RetrieveSecureChannelSessionAsync(string sessionIdentifier, ServiceProxyDataRepository repository)
        {
            return Task.FromResult(repository.Query<SecureChannelSession>(secureChannelSession => secureChannelSession.Identifier == sessionIdentifier).FirstOrDefault());
        }

        public async Task EndSecureChannelSessionAsync(string sessionIdentifier)
        {
            await EndSecureChannelSessionAsync(sessionIdentifier, ServiceProxyDataRepository);
        }

        public async Task EndSecureChannelSessionAsync(string sessionIdentifier, ServiceProxyDataRepository repository)
        {
            SecureChannelSession secureChannelSession = repository.Query<SecureChannelSession>(scs => scs.Identifier == sessionIdentifier).FirstOrDefault();
            if (secureChannelSession != null)
            {
                secureChannelSession.SymmetricKey = null;
                secureChannelSession.SymmetricIV = null;
                secureChannelSession.AsymmetricKey = null;
                secureChannelSession.Expires = new DateTime(1900, 1, 1);
                secureChannelSession.Deleted = DateTime.UtcNow;
                await repository.SaveAsync(secureChannelSession);
            }
        }

        public async Task SetSessionKeyAsync(IHttpContext httpContext, SetSessionKeyRequest setSessionKeyRequest)
        {
            SecureChannelSession secureChannelSession = await GetSecureChannelSessionForContextAsync(httpContext);
            secureChannelSession.SetSymmetricKey(setSessionKeyRequest);
            await ServiceProxyDataRepository.SaveAsync(secureChannelSession);
        }

        protected string GetSecureChannelSessionIdFromCookie(IRequest request)
        {
            Cookie secureChannelSessionCookie = request.Cookies[SecureChannelSession.CookieName];
            if (secureChannelSessionCookie != null)
            {
                return secureChannelSessionCookie.Value;
            }
            return null;
        }

        protected string GetSecureChannelSessionIdFromHeader(IRequest request)
        {
            return request.Headers[Headers.SecureChannelSessionId];            
        }
    }
}
