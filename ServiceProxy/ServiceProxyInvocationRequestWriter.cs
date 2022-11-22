using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvocationRequestWriter : IServiceProxyInvocationRequestWriter
    {
        public ServiceProxyInvocationRequestWriter()
        {
            this.HttpMethods = new Dictionary<ServiceProxyVerbs, HttpMethod>()
            {
                { ServiceProxyVerbs.Get, HttpMethod.Get },
                { ServiceProxyVerbs.Post, HttpMethod.Post }
            };
        }

        protected Dictionary<ServiceProxyVerbs, HttpMethod> HttpMethods
        {
            get;
            set;
        }

        public virtual async Task<HttpRequestMessage> WriteRequestMessageAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            HttpRequestMessage httpRequestMessage = await CreateServiceProxyInvocationRequestMessageAsync(serviceProxyInvocationRequest);
            ServiceProxyInvocationRequestArgumentWriter argumentWriter = serviceProxyInvocationRequest.ServiceProxyInvocationRequestArgumentWriter;
            argumentWriter.WriteArguments(httpRequestMessage);
            return httpRequestMessage;
        }

        protected virtual Task<HttpRequestMessage> CreateServiceProxyInvocationRequestMessageAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethods[serviceProxyInvocationRequest.Verb], serviceProxyInvocationRequest.GetInvocationUrl());
            request.Headers.Add(Web.Headers.ProcessMode, ProcessMode.Current.Mode.ToString());
            request.Headers.Add(Web.Headers.ProcessLocalIdentifier, Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor.LocalIdentifier);
            request.Headers.Add(Web.Headers.ProcessDescriptor, Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor.Current.ToString());

            return Task.FromResult(request);
        }
    }
}
