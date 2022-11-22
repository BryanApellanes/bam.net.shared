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
    public class ServiceProxyInvocationReader : IServiceProxyInvocationReader
    {
        public ServiceProxyInvocationReader(ISecureChannelSessionDataManager secureChannelSessionDataManager, ILogger logger = null)
        {
            this.Logger = logger;
            this.DefaultArgumentReader = new QueryStringServiceProxyInvocationArgumentReader();
            this.ArgumentReaders = new Dictionary<HttpMethodContentTypeKey, ServiceProxyInvocationArgumentReader>()
            {
                { new HttpMethodContentTypeKey("GET"), DefaultArgumentReader },
                { new HttpMethodContentTypeKey("POST", MediaTypes.Json), new InputStreamServiceProxyInvocationArgumentReader() },
                { new HttpMethodContentTypeKey("POST", MediaTypes.BamPipeline), new EncryptedServiceProxyInvocationArgumentReader(secureChannelSessionDataManager) },
            };
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

        public ServiceProxyInvocation ReadServiceProxyInvocation(ServiceProxyPath serviceProxyPath, WebServiceProxyDescriptors webServiceProxyDescriptors, IHttpContext context)
        {
            IRequest request = context.Request;
            string className = serviceProxyPath.Path.ReadUntil('/', out string methodName);

            ServiceProxyInvocationArgumentReader argumentReader = GetArgumentReader(request);

            ServiceProxyInvocation serviceProxyInvocation = argumentReader.CreateServiceProxyInvocation(webServiceProxyDescriptors, className, methodName, context);

            serviceProxyInvocation.Arguments = argumentReader.ReadArgumentsAsync(serviceProxyInvocation.MethodInfo, context).Result;

            return serviceProxyInvocation;
        }
    }
}
