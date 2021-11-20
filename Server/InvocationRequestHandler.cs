using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Net.ServiceProxy;
using Bam.Net.Services;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public class InvocationRequestHandler : RequestHandler
    {
        public InvocationRequestHandler(ServiceProxyResponder serviceProxyResponder)
        {
            this.ServiceProxyResponder = serviceProxyResponder;
        }

        public BamConf BamConf
        {
            get => ServiceProxyResponder?.BamConf;
        }

        public IApplicationNameResolver ApplicationNameResolver
        { 
            get; 
        }

        public ServiceProxyResponder ServiceProxyResponder 
        {
            get;
            private set;
        }

        protected override IHandleRequestResult HandleRequest(IRequest request)
        {
            WebServiceProxyDescriptors webServiceProxyDescriptors = GetWebServiceProxyDescriptors();

            // read the content type to determine if it is encrypted
            // if it is not encrypted
            // resolve using request.Uri
            // if it is encrypted 
            // the input stream should have an encrypted SecureChannelMessage in the body
            //request.InputStream
            //ServiceProxyInvocation serviceProxyInvocation = 
            throw new NotImplementedException();
        }

        protected WebServiceProxyDescriptors GetWebServiceProxyDescriptors()
        {
            return GetWebServiceProxyDescriptors(ApplicationNameResolver.GetApplicationName());
        }

        protected WebServiceProxyDescriptors GetWebServiceProxyDescriptors(string applicationName)
        {
            WebServiceRegistry webServiceRegistry = new WebServiceRegistry();
            
            HashSet<ProxyAlias> proxyAliases = new HashSet<ProxyAlias>();
            BamConf.ProxyAliases.Each(proxyAlias => proxyAliases.Add(proxyAlias));

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

            return new WebServiceProxyDescriptors
            {
                WebServiceRegistry = webServiceRegistry,
                ProxyAliases = proxyAliases
            };
        }

        private void AddProxyAliases(Incubator source, HashSet<ProxyAlias> hashSetToPopulate)
        {
            if(source == null)
            {
                return;
            }
            foreach(string className in source.ClassNames)
            {
                Type currentType = source[className];
                if(currentType.HasCustomAttributeOfType(out ProxyAttribute proxyAttribute))
                {
                    if(!string.IsNullOrEmpty(proxyAttribute.VarName) && !proxyAttribute.VarName.Equals(currentType.Name))
                    {
                        hashSetToPopulate.Add(new ProxyAlias(proxyAttribute.VarName, currentType));
                    }
                }
            }
        }
    }
}
