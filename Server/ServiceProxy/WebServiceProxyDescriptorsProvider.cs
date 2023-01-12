using Bam.Net.Incubation;
using Bam.Net.ServiceProxy;
using Bam.Net.Services;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server.ServiceProxy
{
    /// <summary>
    /// Provides WebServiceProxyDescriptors for a ServiceProxyResponder.
    /// </summary>
    public class WebServiceProxyDescriptorsProvider : IWebServiceProxyDescriptorsProvider
    {
        public WebServiceProxyDescriptorsProvider(ServiceProxyResponder serviceProxyResponder)
        {
            ApplicationWebServiceProxyDescriptors = new Dictionary<string, WebServiceProxyDescriptors>();
            ServiceProxyResponder = serviceProxyResponder;
        }

        public ServiceProxyResponder ServiceProxyResponder { get; }

        public BamConf BamConf
        {
            get => ServiceProxyResponder?.BamConf;
        }

        protected Dictionary<string, WebServiceProxyDescriptors> ApplicationWebServiceProxyDescriptors
        {
            get;
            private set;
        }

        object _webServiceRegistriesLock = new object();
        public WebServiceProxyDescriptors GetWebServiceProxyDescriptors(string applicationName)
        {
            if (!ApplicationWebServiceProxyDescriptors.ContainsKey(applicationName))
            {
                lock (_webServiceRegistriesLock)
                {
                    WebServiceRegistry webServiceRegistry = new WebServiceRegistry();

                    HashSet<ProxyAlias> proxyAliases = new HashSet<ProxyAlias>();
                    BamConf?.ProxyAliases?.Each(proxyAlias => proxyAliases.Add(proxyAlias));

                    AddProxyAliases(ServiceProxySystem.Incubator, proxyAliases);
                    webServiceRegistry.CopyFrom(ServiceProxySystem.Incubator, true);

                    AddProxyAliases(ServiceProxyResponder?.CommonServiceProvider, proxyAliases);
                    webServiceRegistry.CopyFrom(ServiceProxyResponder?.CommonServiceProvider, true);

                    Dictionary<string, Incubator> appServiceProviders = ServiceProxyResponder?.AppServiceProviders;
                    if (appServiceProviders.ContainsKey(applicationName))
                    {
                        Incubator appServices = appServiceProviders[applicationName];
                        AddProxyAliases(appServices, proxyAliases);
                        webServiceRegistry.CopyFrom(appServices, true);
                    }

                    ApplicationWebServiceProxyDescriptors.Add(applicationName, new WebServiceProxyDescriptors
                    {
                        ApplicationServiceRegistry = ServiceProxyResponder?.ApplicationServiceRegistry,
                        WebServiceRegistry = webServiceRegistry,
                        ProxyAliases = proxyAliases
                    });
                }
            }

            return ApplicationWebServiceProxyDescriptors[applicationName];
        }
        private void AddProxyAliases(Incubator source, HashSet<ProxyAlias> hashSetToPopulate)
        {
            if (source == null || hashSetToPopulate == null)
            {
                return;
            }

            foreach (string className in source.ClassNames)
            {
                Type currentType = source[className];
                if (currentType.HasCustomAttributeOfType(out ProxyAttribute proxyAttribute))
                {
                    if (!string.IsNullOrEmpty(proxyAttribute.VarName) && !proxyAttribute.VarName.Equals(currentType.Name))
                    {
                        hashSetToPopulate.Add(new ProxyAlias(proxyAttribute.VarName, currentType));
                    }
                }
            }
        }
    }
}
