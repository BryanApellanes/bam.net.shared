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
    public abstract class ServiceProxyClient
    {
        public const string JsonMediaType = "application/json; charset=utf-8";
        public const string JsonArgsKey = "jsonArgs";

        public ServiceProxyClient()
            : base() 
        {
            this.HttpClient = new HttpClient();
            this.Headers = new Dictionary<string, string>()
            {
                { "User-Agent", UserAgents.ServiceProxyClient() }
            };
            this.MethodUrlFormat = "{BaseAddress}serviceproxy/{ClassName}/{MethodName}?{Parameters}";
            this.HttpMethods = new Dictionary<ServiceProxyVerbs, HttpMethod>()
            {
                { ServiceProxyVerbs.Get, HttpMethod.Get },
                { ServiceProxyVerbs.Post, HttpMethod.Post }
            };
        }

        public ServiceProxyClient(string baseAddress)
            : this()
        {
            this.BaseAddress = baseAddress;            
        }

        public event EventHandler<ServiceProxyInvokeEventArgs> PostStarted;
        public event EventHandler<ServiceProxyInvokeEventArgs> PostComplete;
        public event EventHandler<ServiceProxyInvokeEventArgs> GetStarted;
        public event EventHandler<ServiceProxyInvokeEventArgs> GetComplete;

        /// <summary>
        /// Fires the Getting event 
        /// </summary>
        /// <param name="args"></param>
        protected void OnGetStarted(ServiceProxyInvokeEventArgs args)
        {
            if (GetStarted != null)
            {
                GetStarted(this, args);
            }
        }
        public event EventHandler<ServiceProxyInvokeEventArgs> GetCanceled;
        protected void OnGetCanceled(ServiceProxyInvokeEventArgs args)
        {
            if (GetCanceled != null)
            {
                GetCanceled(this, args);
            }

            OnInvokeMethodCanceled(args);
        }

        /// <summary>
        /// Fires the Got event
        /// </summary>
        /// <param name="args"></param>
        protected void OnGot(ServiceProxyInvokeEventArgs args)
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
        protected void OnPosting(ServiceProxyInvokeEventArgs args)
        {
            if (PostStarted != null)
            {
                PostStarted(this, args);
            }
        }

        public event EventHandler<ServiceProxyInvokeEventArgs> PostCanceled;
        protected void OnPostCanceled(ServiceProxyInvokeEventArgs args)
        {
            if (PostCanceled != null)
            {
                PostCanceled(this, args);
            }

            OnInvokeMethodCanceled(args);
        }

        /// <summary>
        /// Fires the Got event
        /// </summary>
        /// <param name="args"></param>
        protected void OnPosted(ServiceProxyInvokeEventArgs args)
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

        public string MethodUrlFormat
        {
            get;
            set;
        }

        public string BaseAddress
        {
            get;
            set;
        }

        protected Dictionary<string, string> Headers
        {
            get;
            set;
        }
        IApiArgumentProvider _apiArgumentProvider;
        object _apiArgumentProviderLock = new object();
        public IApiArgumentProvider ApiArgumentProvider
        {
            get
            {
                return _apiArgumentProviderLock.DoubleCheckLock(ref _apiArgumentProvider, () => new DefaultApiArgumentProvider());
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

        /// <summary>
        /// The class responsible for providing the name of the
        /// current application.
        /// </summary>
        public IApplicationNameProvider ClientApplicationNameProvider { get; set; }

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

        protected HttpClient HttpClient { get; set; }

        /// <summary>
        /// The event that will occur if an exception occurs during
        /// method invocation
        /// </summary>
        public event EventHandler<ServiceProxyInvokeEventArgs> InvocationException;
        protected void OnInvocationException(ServiceProxyInvokeEventArgs args)
        {
            InvocationException?.Invoke(this, args);
        }

        public event EventHandler<ServiceProxyInvokeEventArgs> InvokeMethodStarted;
        protected void OnInvokeMethodStarted(ServiceProxyInvokeEventArgs args)
        {
            InvokeMethodStarted?.Invoke(this, args);
        }

        public event EventHandler<ServiceProxyInvokeEventArgs> InvokeMethodComplete;
        protected void OnInvokeMethodComplete(ServiceProxyInvokeEventArgs args)
        {
            InvokeMethodComplete?.Invoke(this, args);
        }

        public event EventHandler<ServiceProxyInvokeEventArgs> InvokeMethodCanceled;
        protected void OnInvokeMethodCanceled(ServiceProxyInvokeEventArgs args)
        {
            InvokeMethodCanceled?.Invoke(this, args);
        }

        public async Task<TResult> ReceiveServiceMethodResponseAsync<TService, TResult>(string methodName, params object[] arguments)
        {
            try
            {
                ServiceProxyInvokeRequest<TService> request = new ServiceProxyInvokeRequest<TService>() { MethodName = methodName, Arguments = arguments };
                return await request.ExecuteAsync<TService, TResult>(this);
            }
            catch (Exception ex)
            {
                ServiceProxyInvokeEventArgs<TService> args = new ServiceProxyInvokeEventArgs<TService>() { ClassName = typeof(TService).Name, MethodName = methodName, Arguments = arguments };
                args.Exception = ex;
                OnInvocationException(args);
            }
            return default;
        }

        protected virtual internal Task<HttpRequestMessage> CreateServiceProxyRequestMessageAsync(ServiceProxyVerbs verb, string className, string methodName, string queryStringParameters = "")
        {
            string methodUrl = MethodUrlFormat.NamedFormat(new { BaseAddress, Verb = verb, ClassName = className, MethodName = methodName, Parameters = queryStringParameters });
            HttpRequestMessage request = new HttpRequestMessage(HttpMethods[verb], methodUrl);
            request.Headers.Add("User-Agent", UserAgent);
            request.Headers.Add(Web.Headers.ProcessMode, ProcessMode.Current.Mode.ToString());
            request.Headers.Add(Web.Headers.ProcessLocalIdentifier, Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor.LocalIdentifier);
            Headers.Keys.Where(k => !k.Equals("User-Agent")).Each(key =>
            {
                request.Headers.Add(key, Headers[key]);
            });
            return Task.FromResult(request);
        }

        public abstract Task<string> InvokeServiceMethodAsync(string baseAddress, string className, string methodName, object[] arguments);

        public abstract Task<string> InvokeServiceMethodAsync(string className, string methodName, object[] arguments);

        public abstract Task<string> InvokeServiceMethodAsync(string methodName, object[] arguments);
    }
}
