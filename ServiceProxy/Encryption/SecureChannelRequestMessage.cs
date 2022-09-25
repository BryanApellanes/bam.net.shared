using Bam.Net.CoreServices;
using Bam.Net.Server.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class SecureChannelRequestMessage
    {
        public SecureChannelRequestMessage() { }

        public SecureChannelRequestMessage(ServiceProxyInvocationRequest serviceProxyInvokeRequest)
        {
            this.ClassName = serviceProxyInvokeRequest.ClassName;
            this.MethodName = serviceProxyInvokeRequest.MethodName;
            this.JsonArgs = serviceProxyInvokeRequest.ServiceProxyInvocationRequestArgumentWriter.GetJsonArgumentsArray().ToJson();
        }

        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string JsonArgs { get; set; }

        internal ServiceProxyInvocation ToServiceProxyInvocation(WebServiceProxyDescriptors serviceRegistry, ServiceProxyInvocationArgumentReader serviceProxyInvocationArgumentReader)
        {
            ServiceProxyInvocation serviceProxyInvocation = new ServiceProxyInvocation(serviceRegistry, this.ClassName, this.MethodName);
            string[] jsonArgsArray = JsonArgs.FromJson<string[]>();
            List<ServiceProxyInvocationArgument> args = new List<ServiceProxyInvocationArgument>();
            for (int i = 0; i < serviceProxyInvocation.ParameterInfos.Length; i++)
            {
                args.Add(serviceProxyInvocationArgumentReader.DecodeArgument(serviceProxyInvocation.ParameterInfos[i], jsonArgsArray[i]));
            }
            serviceProxyInvocation.Arguments = args.ToArray();
            return serviceProxyInvocation;
        } 
    }
}
