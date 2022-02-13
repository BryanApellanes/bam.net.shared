using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Net.Server.PathHandlers;
using Bam.Net.ServiceProxy;
using Bam.Net.Services;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server.ServiceProxy
{
    public class ServiceProxyContextHandler : ResponderContextHandler<ServiceProxyResponder>
    {
        public ServiceProxyContextHandler() : base()
        {
            ApplicationWebServiceProxyDescriptors = new Dictionary<string, WebServiceProxyDescriptors>();
            SetHttpMethodHandlers();
        }

        public ServiceProxyResponder ServiceProxyResponder
        {
            get => this.Responder;
        }

        public BamConf BamConf
        {
            get => ServiceProxyResponder?.BamConf;
        }

        public IApplicationNameResolver ApplicationNameResolver
        {
            get => ServiceProxyResponder?.ApplicationNameResolver;
        }

        protected IServiceProxyInvocationResolver ServiceProxyInvocationResolver
        {
            get => ServiceProxyResponder?.ServiceProxyInvocationResolver;
        }

        protected HttpMethodHandlers HttpMethodHandlers { get; private set; }

        protected Dictionary<string, WebServiceProxyDescriptors> ApplicationWebServiceProxyDescriptors
        {
            get;
            private set;
        }

        protected override IHttpResponse HandleContext(IHttpContext context)
        {
            return HttpMethodHandlers.HandleRequest(context);
        }

        protected void SetHttpMethodHandlers()
        {
            HttpMethodHandlers = new HttpMethodHandlers();
            HttpMethodHandlers.SetHandler("Get", (context) =>
            {
                ServiceProxyInvocation serviceProxyInvocation = ResolveServiceProxyInvocation(context);

                bool success = serviceProxyInvocation.Execute(out object result);
                if (success)
                {
                    return new HttpResponse(result.ToJson(), 200);
                }

                return new HttpErrorResponse(serviceProxyInvocation.Exception) { StatusCode = 500 };
            });

            HttpMethodHandlers.SetHandler("Post", (context) =>
            {
                throw new NotImplementedException();
            });
            // POST read content type
            //   json unencrypted invocation request
            //      - read Uri to determine invoke target
            //      - read body to determine method arguments

            //   asym cipher is set key request
            //      - target is SecureChannel
            //      - decrypt body and read as SecureChannelMessage

            //   sym cipher is encypted invocation request
            //      - target is SecureChannel
            //      - decrypt body and read as SecureChannelMessage
        }

        protected WebServiceProxyDescriptors GetWebServiceProxyDescriptors(IRequest request)
        {
            return GetWebServiceProxyDescriptors(ApplicationNameResolver.ResolveApplicationName(request));
        }

        object _webServiceRegistriesLock = new object();
        protected WebServiceProxyDescriptors GetWebServiceProxyDescriptors(string applicationName)
        {
            if (!ApplicationWebServiceProxyDescriptors.ContainsKey(applicationName))
            {
                lock (_webServiceRegistriesLock)
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

                    ApplicationWebServiceProxyDescriptors.Add(applicationName, new WebServiceProxyDescriptors
                    {
                        WebServiceRegistry = webServiceRegistry,
                        ProxyAliases = proxyAliases
                    });
                }
            }

            return ApplicationWebServiceProxyDescriptors[applicationName];
        }

        public override object Clone()
        {
            ServiceProxyContextHandler serviceProxyInvocationRequestHandler = new ServiceProxyContextHandler { Responder = this.ServiceProxyResponder };
            serviceProxyInvocationRequestHandler.CopyProperties(this);
            serviceProxyInvocationRequestHandler.CopyEventHandlers(this);
            return serviceProxyInvocationRequestHandler;
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

        private ServiceProxyInvocation ResolveServiceProxyInvocation(IHttpContext context)
        {
            IRequest request = context.Request;
            WebServiceProxyDescriptors webServiceProxyDescriptors = GetWebServiceProxyDescriptors(request);

            ServiceProxyPath serviceProxyPath = NamedPath as ServiceProxyPath;
            if (serviceProxyPath == null)
            {
                serviceProxyPath = ServiceProxyPath.FromUri(request.Url);
            }

            ServiceProxyInvocation serviceProxyInvocation = ServiceProxyInvocationResolver.ResolveServiceProxyInvocation(serviceProxyPath, webServiceProxyDescriptors, context);
            return serviceProxyInvocation;
        }
    }
}
