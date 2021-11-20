/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using NCuid;
using System.Net.Http;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvokeEventArgs<TService>: ServiceProxyInvokeEventArgs
    {
        public ServiceProxyInvokeEventArgs(ServiceProxyInvokeRequest serviceProxyInvokeRequest) : base(serviceProxyInvokeRequest)
        {
            this.InvokeRequest = serviceProxyInvokeRequest;
            this.Cuid = NCuid.Cuid.Generate();
        }

        public ServiceProxyInvokeEventArgs(ServiceProxyInvokeRequest serviceProxyInvokeRequest, bool cancelInvoke = false) : this(serviceProxyInvokeRequest)
        {
            this.CancelInvoke = cancelInvoke;
        }

        public new ServiceProxyClient<TService> Client
        {
            get;
            set;
        }
    }

    public class ServiceProxyInvokeEventArgs: EventArgs
    {
        public ServiceProxyInvokeEventArgs(ServiceProxyInvokeRequest serviceProxyInvokeRequest)
        {
            this.InvokeRequest = serviceProxyInvokeRequest;
            this.Cuid = NCuid.Cuid.Generate();
        }

        public ServiceProxyInvokeRequest InvokeRequest { get; set; }

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
            get => InvokeRequest.Cuid;
            internal set => InvokeRequest.Cuid = value;
        }

        public bool CancelInvoke
        {
            get;
            internal set;
        }

        public string BaseAddress
        {
            get => InvokeRequest.BaseAddress;
            internal set => InvokeRequest.BaseAddress = value;
        }

        public string ClassName
        {
            get => InvokeRequest.ClassName;
            internal set => InvokeRequest.ClassName = value;
        }

        public string MethodName
        {
            get => InvokeRequest.MethodName;
            internal set => InvokeRequest.MethodName = value;
        }

        public object[] Arguments
        {
            get => InvokeRequest.Arguments;
            internal set => InvokeRequest.Arguments = value;
        }
    }
}
