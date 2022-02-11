using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Net.Logging;
using Bam.Net.Server.PathHandlers;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Encryption;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server.ServiceProxy
{
    /// <summary>
    /// Class used to resolve ServiceProxyInvocations from inbound requests.
    /// </summary>
    public class ServiceProxyInvocationResolver : IServiceProxyInvocationResolver
    {
        public ServiceProxyInvocationResolver(ILogger logger = null)
        {
            this.Logger = logger;
            this.DefaultArgumentReader = new QueryStringServiceProxyInvocationArgumentReader();
            this.ArgumentReaders = new Dictionary<HttpMethodContentTypeKey, ServiceProxyInvocationArgumentReader>()
            {
                { new HttpMethodContentTypeKey("GET"), DefaultArgumentReader },
                { new HttpMethodContentTypeKey("POST", ServiceProxyInvocationRequestArguments.JsonMediaType), new InputStreamServiceProxyInvocationArgumentReader() },
                { new HttpMethodContentTypeKey("POST", SecureChannelRequestMessage.SymetricCipherMediaType), new SecureServiceProxyInvocationArgumentReader() },
            };
            // CREATE A CUSTOM ENCODING PIPELINE ARGUMENT READER
            // IMPLEMENT IValueTransformer that takes a list of encoding names,
            // for example: json, aes, gzip // move this note to the data management related namespace(s)
            // or
            // json, rsa, gzip
            // use internally from ApiArgumentEncoder and ...ArgumentReader

            //   asym cipher is set key request
            //      - target is SecureChannel
            //      - decrypt body and read as SecureChannelMessage

            //   sym cipher is encypted invocation request
            //      - target is SecureChannel
            //      - decrypt body and read as SecureChannelMessage
        }

        protected Dictionary<HttpMethodContentTypeKey, ServiceProxyInvocationArgumentReader> ArgumentReaders { get; }

        protected ServiceProxyInvocationArgumentReader DefaultArgumentReader { get; }

        protected ServiceProxyInvocationArgumentReader GetArgumentReader(IRequest request)
        {
            HttpMethodContentTypeKey key = new HttpMethodContentTypeKey(request);
            if (ArgumentReaders.ContainsKey(key))
            {
                return ArgumentReaders[key];
            }
            return DefaultArgumentReader;
        }

        public ILogger Logger { get; set; }

        public ServiceProxyInvocation ResolveServiceProxyInvocation(ServiceProxyPath serviceProxyPath, WebServiceProxyDescriptors webServiceProxyDescriptors, IHttpContext context)
        {
            IRequest request = context.Request;
            string className = serviceProxyPath.Path.ReadUntil('/', out string methodName);

            ServiceProxyInvocation serviceProxyInvocation = new ServiceProxyInvocation(webServiceProxyDescriptors, className, methodName, context);

            ServiceProxyInvocationArgumentReader argumentReader = GetArgumentReader(request);

            argumentReader.SetArguments(serviceProxyInvocation, request);

            return serviceProxyInvocation;
        }
    }
}
