using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Net.Logging;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    /// <summary>
    /// Class used to determine what method to execute for a given HttpContext.
    /// </summary>
    public class ServiceProxyInvocationResolver: IServiceProxyInvocationResolver
    {
        public ServiceProxyInvocationResolver(ILogger logger = null)
        {
            Logger = logger;
        }

        public ServiceProxyInvocationResolver(ServiceRegistry serviceProvider, params ProxyAlias[] aliases)
        {
            ServiceProvider = serviceProvider;
            //HttpContext = context;
            ProxyAliases = aliases;
        }

        public ServiceRegistry ServiceProvider { get; set; }
        //public IHttpContext HttpContext { get; set; }
        public ProxyAlias[] ProxyAliases { get; set; }
        public ILogger Logger { get; set; }
/*        public virtual ExecutionRequest ResolveExecutionRequest()
        {
            Args.ThrowIfNull(ServiceProvider, $"{nameof(ExecutionRequestResolver)}.{nameof(ServiceProvider)}");
            Args.ThrowIfNull(HttpContext, $"{nameof(ExecutionRequestResolver)}.{nameof(HttpContext)}");
            Args.ThrowIfNull(ProxyAliases, $"{nameof(ExecutionRequestResolver)}.{nameof(ProxyAliases)}");
            return ResolveExecutionRequest(HttpContext, ServiceProvider, ProxyAliases);
        }*/

        /// <summary>
        /// Analyzes the specified IHttpContext, Incubator and ProxyAlias array
        /// and returns an ExecutionRequest configured and ready for execution.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="aliases"></param>
        /// <returns></returns>
        public virtual ServiceProxyInvocation ResolveInvocationRequest(IHttpContext httpContext, Incubator serviceProvider, params ProxyAlias[] aliases)
        {
            // TODO: refactor this method to analyze httpContext.Request.ContentType
            // See ExecutionRequest.GetArguments see commit (2526558ea460852c033d1151dc190308a9feaefd)
            ServiceProxyInvocation execRequest = new ServiceProxyInvocation(httpContext, serviceProvider, aliases)
            {
                Logger = Logger ?? Log.Default
            };            
            
            ServiceProxyInvocation.DecryptSecureChannelInvoke(execRequest);
            return execRequest;
        }   
        
        public virtual InvocationTargetInfo ResolveInvocationTarget(IHttpContext context)
        {
            return InvocationTargetInfo.ResolveInvocationTarget(context.Request.Url.AbsolutePath, ServiceProvider, ProxyAliases);
        }

        
    }
}
