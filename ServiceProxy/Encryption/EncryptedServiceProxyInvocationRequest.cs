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
                MethodName = nameof(SecureChannel.Execute),
                QueryStringArguments = string.Empty,
            });
        }
    }
}
