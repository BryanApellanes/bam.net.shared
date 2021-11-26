﻿using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Net.Logging;
using Bam.Net.Server.PathHandlers;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
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
            this.ArgumentReaders = new Dictionary<ContentTypeMethod, ServiceProxyInvocationArgumentReader>()
            {
                { new ContentTypeMethod("GET"), DefaultArgumentReader },
                { new ContentTypeMethod("POST", ServiceProxyInvocationRequestArguments.JsonMediaType), new InputStreamServiceProxyInvocationArgumentReader() },
            };

            //   asym cipher is set key request
            //      - target is SecureChannel
            //      - decrypt body and read as SecureChannelMessage

            //   sym cipher is encypted invocation request
            //      - target is SecureChannel
            //      - decrypt body and read as SecureChannelMessage
        }

        protected Dictionary<ContentTypeMethod, ServiceProxyInvocationArgumentReader> ArgumentReaders { get; }

        protected ServiceProxyInvocationArgumentReader DefaultArgumentReader { get; }

        protected ServiceProxyInvocationArgumentReader GetArgumentReader(IRequest request)
        {
            ContentTypeMethod key = new ContentTypeMethod(request);
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
