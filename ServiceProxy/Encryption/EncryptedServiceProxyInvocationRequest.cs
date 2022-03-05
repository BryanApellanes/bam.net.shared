using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class EncryptedServiceProxyInvocationRequest : ServiceProxyInvocationRequest
    {
        public EncryptedServiceProxyInvocationRequest() { }

        public EncryptedServiceProxyInvocationRequest(ServiceProxyClient serviceProxyClient, string className, string methodName, params object[] arguments) : base(serviceProxyClient, className, methodName, arguments)
        {
        }

        ServiceProxyInvocationRequestArgumentWriter _serviceProxyArgumentWriter;
        public override ServiceProxyInvocationRequestArgumentWriter ServiceProxyInvocationRequestArgumentWriter
        {
            get
            {
                if (_serviceProxyArgumentWriter == null)
                {
                    _serviceProxyArgumentWriter = new EncryptedServiceProxyInvocationRequestArgumentWriter(this);
                }
                return _serviceProxyArgumentWriter;
            }
        }

        public override ServiceProxyVerbs Verb 
        {
            get { return ServiceProxyVerbs.Post; } // always POST
            set
            {
                // read only
            }
        }

        public override string GetInvocationUrl(bool includeQueryString = true, ServiceProxyClient serviceProxyClient = null)
        {
            return MethodUrlFormat.NamedFormat(new
            {
                BaseAddress = serviceProxyClient?.BaseAddress ?? BaseAddress,
                ClassName = nameof(SecureChannel),
                MethodName = "Invoke",
                QueryStringArguments = "",
            });
        }
    }
}
