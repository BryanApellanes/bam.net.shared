using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Logging;
using Bam.Net.Server;
using Bam.Net.CoreServices;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Encryption;

namespace Bam.Net.Services
{
    /// <summary>
    /// Used to enable the AsyncCallbackService.
    /// </summary>
    public class ServiceProxyServer : SimpleServer<ServiceProxyResponder>
    {
        public ServiceProxyServer(ServiceRegistry serviceRegistry, ILogger logger = null) : this(serviceRegistry,
            new ServiceProxyResponder(new BamConf(), logger), logger)
        {
        }

        public ServiceProxyServer(ServiceRegistry serviceRegistry, ServiceProxyResponder responder, ILogger logger) : base(responder, logger)
        {
            ServiceSubdomains = new HashSet<ServiceSubdomainAttribute>();
            RegisterServices(serviceRegistry);            
        }

        public void RegisterServices(ServiceRegistry serviceRegistry, bool requireApiKeyResolver = false)
        {
            ServiceRegistry = new WebServiceRegistry(serviceRegistry);
            Responder.ClearCommonServices();
            Responder.ClearAppServices();
            Responder.SetCommonWebServices(ServiceRegistry);
            foreach(Type type in ServiceRegistry.MappedTypes)
            {
                if(type.HasCustomAttributeOfType(out ServiceSubdomainAttribute attr))
                {
                    ServiceSubdomains.Add(attr);
                }
            }
            SetApiHmacKeyResolver(serviceRegistry, requireApiKeyResolver ? ApiHmacKeyResolver.Default : null);
        }

        public override void Start()
        {
            HostBinding[] copy = new HostBinding[HostBindings.Count];
            HostBindings.CopyTo(copy);
            ServiceSubdomains.Each(sub =>
            {
                copy.Each(hp =>
                {
                    HostBindings.Add(hp.FromServiceSubdomain(sub));
                });
            });
            base.Start();
        }

        public HashSet<ServiceSubdomainAttribute> ServiceSubdomains { get; set; }
        protected WebServiceRegistry ServiceRegistry { get; set; }

        protected void SetApiHmacKeyResolver(ServiceRegistry registry, IApiHmacKeyResolver ifNull)
        {
            IApiHmacKeyResolver apiKeyResolver = registry.Get(ifNull);
            Responder.CommonSecureChannel.ApiHmacKeyResolver = apiKeyResolver;
            Responder.AppSecureChannels.Values.Each(sc => sc.ApiHmacKeyResolver = apiKeyResolver);
        }
    }
}
