using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Bam.Net.ServiceProxy.Encryption
{
    internal class EncryptedServiceProxyInvocationHttpRequestContext
    {
        public ValueTransformerPipeline<SecureChannelRequestMessage> Encryptor { get; set; }
        public string PlainPostBody { get; set; }

        public HttpRequestMessage HttpRequestMessage { get; set; }

        public string ContentType { get; set; }

        public Type ServiceType { get; set; }
        public MethodInfo MethodInfo { get; set; }
    }
}
