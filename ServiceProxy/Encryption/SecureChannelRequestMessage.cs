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



    }
}
