using Bam.Net.CoreServices;
using Bam.Net.Encryption;
using Bam.Net.Incubation;
using Bam.Net.Server.PathHandlers;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Data;
using Bam.Net.ServiceProxy.Encryption;
using Bam.Net.Services;
using Bam.Net.Web;
using Bam.NET.Encryption;
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

        ISecureChannelSessionDataManager _secureChannelSessionManager;
        readonly object _secureChannelSessionManagerLock = new object();
        [Inject]
        public ISecureChannelSessionDataManager SecureChannelSessionDataManager
        {
            get
            {
                return _secureChannelSessionManagerLock.DoubleCheckLock(ref _secureChannelSessionManager, () => new SecureChannelSessionDataManager());
            }
            set
            {
                _secureChannelSessionManager = value;
            }
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
            if (IsSecureChannelExecutionRequest(context, out ServiceProxyInvocation serviceProxyInvocation))
            {
                // if it's a securechannel request the type of the result will always be 
                // SecureChannelResponseMessage
                bool success = serviceProxyInvocation.Execute(out SecureChannelResponseMessage secureChannelResponseMessage);

                if (success)
                {
                    return CreateResponse(context, secureChannelResponseMessage, () => EncryptedHttpResponse.ForData(secureChannelResponseMessage, GetClientSessionInfo(context), EncryptionSchemes.Symmetric));
                }
            }
            else
            {
                bool success = serviceProxyInvocation.Execute(out object result);
                if (success)
                {
                    return new HttpResponse(result.ToJson(), 200);
                }
            }

            return new HttpErrorResponse(serviceProxyInvocation.Exception) { StatusCode = 500 };
        }

        public bool IsSecureChannelExecutionRequest(IHttpContext context, out ServiceProxyInvocation serviceProxyInvocation)
        {
            serviceProxyInvocation = ReadServiceProxyInvocation(context);
            return serviceProxyInvocation.TargetType == typeof(SecureChannel) && serviceProxyInvocation.MethodName.Equals(nameof(SecureChannel.Execute));
        }

        protected IHttpResponse CreateResponse<T>(IHttpContext context, T result, Func<IHttpResponse> defaultResponseProvider)
        {
            HashSet<string> acceptTypes = context?.Request.AcceptTypes == null ? new HashSet<string>(): new HashSet<string>(context?.Request?.AcceptTypes);
            if (acceptTypes.Contains(MediaTypes.SymmetricCipher))
            {
                return EncryptedHttpResponse.ForData(result, GetClientSessionInfo(context), EncryptionSchemes.Symmetric);
            }
            if (acceptTypes.Contains(MediaTypes.AsymmetricCipher))
            {
                return EncryptedHttpResponse.ForData(result, GetClientSessionInfo(context), EncryptionSchemes.Asymmetric);
            }
            return defaultResponseProvider();
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

        private ClientSessionInfo GetClientSessionInfo(IHttpContext context)
        {
            SecureChannelSession secureChannelSession = SecureChannelSessionDataManager.GetSecureChannelSessionForContextAsync(context).Result;
            ClientSessionInfo clientSessionInfo = secureChannelSession.GetClientSession(false);
            return clientSessionInfo;
        }
    }
}
