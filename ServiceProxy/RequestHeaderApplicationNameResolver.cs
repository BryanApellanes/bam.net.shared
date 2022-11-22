using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.ServiceProxy;
using Bam.Net.Web;
using Bam.Net.ServiceProxy.Encryption;

namespace Bam.Net.ServiceProxy
{
    /// <summary>
    /// IApplicationNameResolver implementation that reads the application name
    /// from the X-Bam-AppName header if it exists
    /// </summary>
    public class RequestHeaderApplicationNameResolver : IApplicationNameResolver
    {
        public RequestHeaderApplicationNameResolver()
        {
        }

        public RequestHeaderApplicationNameResolver(IHttpContext context)
        {
            HttpContext = context;
        }

        public IHttpContext HttpContext { get; set; }

        public string ResolveApplicationName(IRequest context)
        {
            return ResolveApplicationName(context, true);
        }

        public string ResolveApplicationName(IRequest request, bool withHost)
        {
            string host = request?.Url?.Host;
            string userHostAddress = request?.UserHostAddress;
            string fromHeader = request?.Headers[Headers.ApplicationName];
            string unknown = Bam.Net.CoreServices.ApplicationRegistration.Data.Application.Unknown.Name;            
            return withHost ? $"{fromHeader.Or(unknown)}@{host.Or("localhost")}({userHostAddress})": fromHeader.Or(unknown);
        }

        public string GetApplicationName()
        {
            return ResolveApplicationName(HttpContext.Request, false);
        }

        public static string Resolve(IHttpContext context)
        {
            return new RequestHeaderApplicationNameResolver(context).GetApplicationName();
        }
    }
}
