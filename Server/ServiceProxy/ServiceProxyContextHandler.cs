using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Net.Server.PathHandlers;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Encryption;
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
            SetHttpMethodHandlers();
        }

        public ServiceProxyResponder ServiceProxyResponder
        {
            get => this.Responder;
        }

        public IApplicationNameResolver ApplicationNameResolver
        {
            get => ServiceProxyResponder?.ApplicationNameResolver;
        }

        protected IServiceProxyInvocationReader ServiceProxyInvocationReader
        {
            get => ServiceProxyResponder?.ServiceProxyInvocationReader;
        }

        protected IWebServiceProxyDescriptorsProvider WebServiceProxyDescriptorsProvider
        {
            get => ServiceProxyResponder?.WebServiceProxyDescriptorsProvider;
        }

        protected HttpMethodHandlers HttpMethodHandlers { get; private set; }

        protected override IHttpResponse HandleContext(IHttpContext context)
        {
            return HttpMethodHandlers.HandleRequest(context);
        }

        protected void SetHttpMethodHandlers()
        {
            HttpMethodHandlers = new HttpMethodHandlers();
            HttpMethodHandlers.SetHandler("Get", CreateGetResponse);
            HttpMethodHandlers.SetHandler("Post", ExecuteInvocation);
        }

        protected IHttpResponse CreateGetResponse(IHttpContext context)
        {
            if (ServiceProxyResponder.IsProxyCodeRequest(context))
            {
                return new HttpResponse(ServiceProxyResponder.GetProxyCode(context), 200);
            }
            return ExecuteInvocation(context);
        }

        protected IHttpResponse ExecuteInvocation(IHttpContext context)
        {
            ServiceProxyInvocation serviceProxyInvocation = ReadServiceProxyInvocation(context);
            
            bool success = serviceProxyInvocation.Execute(out object result);
            if (success)
            {
                return new HttpResponse(result.ToJson(), 200);
            }

            return new HttpErrorResponse(serviceProxyInvocation.Exception) { StatusCode = 500 };
        }

        protected WebServiceProxyDescriptors GetWebServiceProxyDescriptors(IRequest request)
        {
            return WebServiceProxyDescriptorsProvider.GetWebServiceProxyDescriptors(ApplicationNameResolver.ResolveApplicationName(request));
        }

        public override object Clone()
        {
            ServiceProxyContextHandler serviceProxyInvocationRequestHandler = new ServiceProxyContextHandler { Responder = this.ServiceProxyResponder };
            serviceProxyInvocationRequestHandler.CopyProperties(this);
            serviceProxyInvocationRequestHandler.CopyEventHandlers(this);
            return serviceProxyInvocationRequestHandler;
        }

        private ServiceProxyInvocation ReadServiceProxyInvocation(IHttpContext context)
        {
            IRequest request = context.Request;
            WebServiceProxyDescriptors webServiceProxyDescriptors = GetWebServiceProxyDescriptors(request);

            ServiceProxyPath serviceProxyPath = NamedPath as ServiceProxyPath;
            if (serviceProxyPath == null)
            {
                serviceProxyPath = ServiceProxyPath.FromUri(request.Url);
            }

            ServiceProxyInvocation serviceProxyInvocation = ServiceProxyInvocationReader.ReadServiceProxyInvocation(serviceProxyPath, webServiceProxyDescriptors, context);
            return serviceProxyInvocation;
        }
    }
}
