using Bam.Net.CoreServices.ApplicationRegistration.Data;
using Bam.Net.CoreServices.ApplicationRegistration.Data.Dao.Repository;
using Bam.Net.ServiceProxy;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.CoreServices
{
    /// <summary>
    /// Resolves a client application name from a request header or domain name
    /// </summary>
    public class ClientApplicationNameResolver : IApplicationNameResolver
    {
        public ClientApplicationNameResolver(ApplicationRegistrationRepository repo)
        {
            ApplicationRegistrationRepository = repo;
        }

/*        public ClientApplicationNameResolver(ApplicationRegistrationRepository repo, IHttpContext context) : this(repo)
        {
            HttpContext = context;
        }*/

        public ApplicationRegistrationRepository ApplicationRegistrationRepository { get; set; }

        // TOOD: pretty sure this will  cause a race condition depending on how this is set and used
        public IHttpContext HttpContext { get; set; }

        public string GetApplicationName()
        {
            return ResolveApplicationName(HttpContext.Request);
        }

        public string ResolveApplicationName(IRequest request)
        {
            string fromHeader = request?.Headers[Headers.ApplicationName];
            if (string.IsNullOrEmpty(fromHeader))
            {
                string domainName = request?.Url?.Host;
                if (!string.IsNullOrEmpty(domainName))
                {
                    HostDomain hostDomain = ApplicationRegistrationRepository.OneHostDomainWhere(d => d.DomainName == domainName);
                    if(hostDomain != null)
                    {
                        return hostDomain.DefaultApplicationName;
                    }
                }
            }
            return fromHeader.Or(Bam.Net.CoreServices.ApplicationRegistration.Data.Application.Unknown.Name);
        }
    }
}
