using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Secure
{
    public class SecureServiceProxyInvokeRequest<TService> : ServiceProxyInvokeRequest
    {
        public SecureServiceProxyInvokeRequest(ServiceProxyInvokeRequest primaryRequest) : base(typeof(TService))
        {
            this.SecureChannelInvokeRequest = new ServiceProxyInvokeRequest(typeof(SecureChannel))
            {
                MethodName = nameof(SecureChannel.Invoke),
                Arguments = new object[] { primaryRequest.ClassName, primaryRequest.MethodName, primaryRequest.ServiceProxyArguments.ArgumentsAsJsonArrayOfJsonStrings }
            };
            this.PrimaryInvokeRequest = primaryRequest;
        }

        /// <summary>
        /// Gets the request intended for the SecureChannel.
        /// </summary>
        public ServiceProxyInvokeRequest SecureChannelInvokeRequest
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the actual invocation request.
        /// </summary>
        public ServiceProxyInvokeRequest PrimaryInvokeRequest
        {
            get;
            private set;
        }
    }
}
