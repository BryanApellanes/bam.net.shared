/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvocationRequestEventArgs<TService>: ServiceProxyInvocationRequestEventArgs
    {
        public ServiceProxyInvocationRequestEventArgs(ServiceProxyInvocationRequest serviceProxyInvokeRequest) : base(serviceProxyInvokeRequest)
        {
            this.InvocationRequest = serviceProxyInvokeRequest;
            this.Cuid = Bam.Net.Cuid.Generate();
        }

        public ServiceProxyInvocationRequestEventArgs(ServiceProxyInvocationRequest serviceProxyInvokeRequest, bool cancelInvoke = false) : this(serviceProxyInvokeRequest)
        {
            this.CancelInvoke = cancelInvoke;
        }

        public new ServiceProxyClient<TService> Client
        {
            get;
            set;
        }
    }

    public class ServiceProxyInvocationRequestEventArgs: EventArgs
    {
        public ServiceProxyInvocationRequestEventArgs(ServiceProxyInvocationRequest serviceProxyInvokeRequest)
        {
            this.InvocationRequest = serviceProxyInvokeRequest;
            this.Cuid = Bam.Net.Cuid.Generate();
        }

        public ServiceProxyInvocationRequest InvocationRequest { get; set; }

        public ServiceProxyClient Client { get; set; }

        /// <summary>
        /// Gets or sets the request message.
        /// </summary>
        public HttpRequestMessage RequestMessage { get; set; }

        public HttpResponseMessage ResponseMessage { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Exception { get; set; }
        
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets a value used to uniquely identify an invocation
        /// when executing event subscriptions.
        /// </summary>
        public string Cuid 
        {
            get => InvocationRequest.Cuid;
            internal set => InvocationRequest.Cuid = value;
        }

        public bool CancelInvoke
        {
            get;
            internal set;
        }

        public string BaseAddress
        {
            get => InvocationRequest.BaseAddress;
            internal set => InvocationRequest.BaseAddress = value;
        }

        public string ClassName
        {
            get => InvocationRequest.ClassName;
            internal set => InvocationRequest.ClassName = value;
        }

        public string MethodName
        {
            get => InvocationRequest.MethodName;
            internal set => InvocationRequest.MethodName = value;
        }

        public object[] Arguments
        {
            get => InvocationRequest.Arguments;
            internal set => InvocationRequest.Arguments = value;
        }
    }
}
