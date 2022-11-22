/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Reflection;
using Bam.Net;
using Bam.Net.Logging;
using Bam.Net.Web;
using Bam.Net.Configuration;
using Bam.Net.Incubation;
using System.Net.Http;

namespace Bam.Net.ServiceProxy
{
    public abstract class ServiceProxyClient : IServiceProxyClient
    {
        public const string DefaultBaseAddress = "http://localhost:8080";

        public ServiceProxyClient(Type serviceType, string baseAddress = DefaultBaseAddress)
            : base() 
        {
            this.ServiceType = serviceType;
            this.BaseAddress = baseAddress;
            this.HttpClient = new HttpClient() { BaseAddress = new Uri(baseAddress) };
            this.Headers = new Dictionary<string, string>()
            {
                { "User-Agent", UserAgents.ServiceProxyClient() }
            };

            this.HttpMethods = new Dictionary<ServiceProxyVerbs, HttpMethod>()
            {
                { ServiceProxyVerbs.Get, HttpMethod.Get },
                { ServiceProxyVerbs.Post, HttpMethod.Post }
            };
        }

        public ServiceProxyClient(HttpClient httpClient, Type serviceType): this(serviceType)
        {
            this.HttpClient = httpClient;
            this.BaseAddress = httpClient.BaseAddress.ToString();
        }

        public event EventHandler<ServiceProxyInvocationRequestEventArgs> PostStarted;
        public event EventHandler<ServiceProxyInvocationRequestEventArgs> PostComplete;
        public event EventHandler<ServiceProxyInvocationRequestEventArgs> PostCanceled;
        public event EventHandler<ServiceProxyInvocationRequestEventArgs> GetStarted;
        public event EventHandler<ServiceProxyInvocationRequestEventArgs> GetComplete;

        public string BaseAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Fires the Getting event 
        /// </summary>
        /// <param name="args"></param>
        protected void OnGetStarted(ServiceProxyInvocationRequestEventArgs args)
        {
            if (GetStarted != null)
            {
                GetStarted(this, args);
            }
        }
        public event EventHandler<ServiceProxyInvocationRequestEventArgs> GetCanceled;
        protected void OnGetCanceled(ServiceProxyInvocationRequestEventArgs args)
        {
            if (GetCanceled != null)
            {
                GetCanceled(this, args);
            }

            OnInvocationCanceled(args);
        }

        /// <summary>
        /// Fires the Got event
        /// </summary>
        /// <param name="args"></param>
        protected void OnGetComplete(ServiceProxyInvocationRequestEventArgs args)
        {
            if (GetComplete != null)
            {
                GetComplete(this, args);
            }
        }

        /// <summary>
        /// Fires the Posting event 
        /// </summary>
        /// <param name="args"></param>
        protected void OnPostStarted(ServiceProxyInvocationRequestEventArgs args)
        {
            if (PostStarted != null)
            {
                PostStarted(this, args);
            }
        }

        protected void OnPostCanceled(ServiceProxyInvocationRequestEventArgs args)
        {
            if (PostCanceled != null)
            {
                PostCanceled(this, args);
            }

            OnInvocationCanceled(args);
        }

        /// <summary>
        /// Fires the `PostComplete` event
        /// </summary>
        /// <param name="args"></param>
        protected void OnPostComplete(ServiceProxyInvocationRequestEventArgs args)
        {
            if (PostComplete != null)
            {
                PostComplete(this, args);
            }
        }

        protected Dictionary<ServiceProxyVerbs, HttpMethod> HttpMethods
        {
            get;
            set;
        }

        public Type ServiceType { get; internal set; }

        protected Dictionary<string, string> Headers
        {
            get;
            set;
        }

        IApiArgumentEncoder _apiArgumentProvider;
        object _apiArgumentProviderLock = new object();
        public IApiArgumentEncoder ApiArgumentEncoder
        {
            get
            {
                return _apiArgumentProviderLock.DoubleCheckLock(ref _apiArgumentProvider, () => new DefaultApiArgumentEncoder() { ServiceType = ServiceType });
            }
            set
            {
                _apiArgumentProvider = value;
            }
        }

        ILogger _logger;
        object _loggerSync = new object();
        public ILogger Logger
        {
            get
            {
                return _loggerSync.DoubleCheckLock(ref _logger, () => Log.Default);
            }
            set
            {
                _logger = value;
            }
        }

        public string UserAgent
        {
            get
            {
                return this.Headers["User-Agent"];
            }
            set
            {
                this.Headers["User-Agent"] = value;
            }
        }
        
        protected string LastResponse { get; set; }

        protected internal HttpClient HttpClient { get; set; }

        /// <summary>
        /// The event that is raised when an exception occurs during method invocation.
        /// </summary>
        public event EventHandler<ServiceProxyInvocationRequestEventArgs> InvocationException;

        protected void OnInvocationException(ServiceProxyInvocationRequestEventArgs args)
        {
            InvocationException?.Invoke(this, args);
        }

        public event EventHandler<ServiceProxyInvocationRequestEventArgs> InvocationStarted;
        protected void OnInvocationStarted(ServiceProxyInvocationRequestEventArgs args)
        {
            InvocationStarted?.Invoke(this, args);
        }

        public event EventHandler<ServiceProxyInvocationRequestEventArgs> InvocationComplete;
        protected void OnInvocationComplete(ServiceProxyInvocationRequestEventArgs args)
        {
            InvocationComplete?.Invoke(this, args);
        }

        public event EventHandler<ServiceProxyInvocationRequestEventArgs> InvocationCanceled;
        protected void OnInvocationCanceled(ServiceProxyInvocationRequestEventArgs args)
        {
            InvocationCanceled?.Invoke(this, args);
        }

        public async Task<string> ReceivePostResponseAsync(string methodName, params object[] arguments)
        {
            return await ReceivePostResponseAsync(new ServiceProxyInvocationRequest(this, ServiceType.Name, methodName, arguments));
        }

        public virtual async Task<string> ReceivePostResponseAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            ServiceProxyInvocationRequestEventArgs args = new ServiceProxyInvocationRequestEventArgs(serviceProxyInvocationRequest);
            args.Client = this;

            OnPostStarted(args);
            string result = string.Empty;
            if (args.CancelInvoke)
            {
                OnPostCanceled(args);
            }
            else
            {
                IServiceProxyInvocationRequestWriter serviceProxyInvocationRequestWriter = GetRequestWriter();
                HttpRequestMessage request = await serviceProxyInvocationRequestWriter.WriteRequestMessageAsync(serviceProxyInvocationRequest);

                try
                {
                    HttpResponseMessage response = await HttpClient.SendAsync(request);
                    args.RequestMessage = request;
                    args.ResponseMessage = response;
                    result = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    OnPostComplete(args);
                }
                catch (Exception ex)
                {
                    args.Exception = ex;
                    OnInvocationException(args);
                }
            }
            return result;
        }

        public async Task<string> ReceiveGetResponseAsync(string methodName, params object[] arguments)
        {
            MethodInfo methodInfo = ServiceType.GetMethod(methodName, arguments.Select(argument => argument.GetType()).ToArray());
            return await ReceiveGetResponseAsync(new ServiceProxyInvocationRequest(this, ServiceType.Name, methodName, arguments));
        }

        public virtual async Task<string> ReceiveGetResponseAsync(ServiceProxyInvocationRequest request)
        {
            ServiceProxyInvocationRequestEventArgs args = request.CopyAs<ServiceProxyInvocationRequestEventArgs>();
            args.Client = this;

            OnGetStarted(args);
            string result = string.Empty;
            if (args.CancelInvoke)
            {
                OnGetCanceled(args);
            }
            else
            {
                HttpRequestMessage requestMessage = await CreateServiceProxyInvocationRequestMessageAsync(request);
                HttpResponseMessage response = await HttpClient.SendAsync(requestMessage);
                args.RequestMessage = requestMessage;
                args.ResponseMessage = response;
                result = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                OnGetComplete(args);
            }
            return result;
        }

        public virtual async Task<HttpRequestMessage> CreateServiceProxyInvocationRequestMessageAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest)
        {
            if (serviceProxyInvocationRequest == null)
            {
                throw new ArgumentNullException(nameof(serviceProxyInvocationRequest));
            }

            if (serviceProxyInvocationRequest.ServiceType == null)
            {
                throw new ArgumentNullException("ServiceType not specified");
            }

            IServiceProxyInvocationRequestWriter requestWriter = GetRequestWriter();

            HttpRequestMessage httpRequestMessage = await requestWriter.WriteRequestMessageAsync(serviceProxyInvocationRequest);
            Headers.Keys.Each(key => httpRequestMessage.Headers.Add(key, Headers[key]));
            return httpRequestMessage;
        }

        protected virtual IServiceProxyInvocationRequestWriter GetRequestWriter()
        {
            return new ServiceProxyInvocationRequestWriter();
        }

        public abstract string InvokeServiceMethod(string methodName, params object[] arguments);

        public abstract Task<string> InvokeServiceMethodAsync(string baseAddress, string className, string methodName, object[] arguments);

        public abstract string InvokeServiceMethod(string className, string methodName, object[] arguments);

        public abstract Task<string> InvokeServiceMethodAsync(string className, string methodName, object[] arguments);

        public abstract Task<string> InvokeServiceMethodAsync(string methodName, object[] arguments);
    }
}
