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
        public const string JsonArgsMemberName = "jsonArgs";
        public const string DefaultBaseAddress = "http://localhost:8080";

        public ServiceProxyClient(Type serviceType, string baseAddress = DefaultBaseAddress)
            : base() 
        {
            this.ServiceType = serviceType;
            this.HttpClient = new HttpClient() { BaseAddress = new Uri(baseAddress) };
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

        public ServiceProxyClient(HttpClient httpClient, Type serviceType): this(serviceType)
        {
            this.HttpClient = httpClient;
            this.BaseAddress = httpClient.BaseAddress.ToString();
        }

        public event EventHandler<ServiceProxyInvokeEventArgs> PostStarted;
        public event EventHandler<ServiceProxyInvokeEventArgs> PostComplete;
        public event EventHandler<ServiceProxyInvokeEventArgs> PostCanceled;
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

        public Type ServiceType { get; internal set; }

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

        protected internal HttpClient HttpClient { get; set; }

        /// <summary>
        /// The event that is raised when an exception occurs during method invocation.
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

        public async Task<string> ReceivePostResponseAsync(string methodName, params object[] arguments)
        {
            return await ReceivePostResponseAsync(new ServiceProxyInvokeRequest() { BaseAddress = BaseAddress, ServiceType = ServiceType, MethodName = methodName, Arguments = arguments });
        }

        public virtual async Task<string> ReceivePostResponseAsync(ServiceProxyInvokeRequest request)
        {
            HttpRequestMessage message = await CreateServiceProxyRequestMessageAsync(ServiceProxyVerbs.Post, request.ClassName, request.MethodName, "nocache=".RandomLetters(4));
            return await ReceivePostResponseAsync(request, message);
        }

        protected virtual async Task<string> ReceivePostResponseAsync(ServiceProxyInvokeRequest invokeRequest, HttpRequestMessage request)
        {
            ServiceProxyInvokeEventArgs args = new ServiceProxyInvokeEventArgs(invokeRequest);
            args.Client = this;

            OnPosting(args);
            string result = string.Empty;
            if (args.CancelInvoke)
            {
                OnPostCanceled(args);
            }
            else
            {
                string jsonArgsMember = ApiArgumentProvider.ArgumentsToJsonArgsMember(args.Arguments);
                Uri uri = new Uri(args.BaseAddress);
                if (HttpClient.BaseAddress == null || !HttpClient.BaseAddress.ToString().Equals(uri.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    HttpClient.BaseAddress = uri;
                }
                SetHttpArgsContent(jsonArgsMember, request);
                HttpResponseMessage response = await HttpClient.SendAsync(request);
                args.RequestMessage = request;
                args.ResponseMessage = response;
                result = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                OnPosted(args);
            }
            return result;
        }

        public async Task<string> ReceiveGetResponseAsync(string methodName, params object[] arguments)
        {
            MethodInfo methodInfo = ServiceType.GetMethod(methodName, arguments.Select(argument => argument.GetType()).ToArray());
            return await ReceiveGetResponseAsync(new ServiceProxyInvokeRequest { BaseAddress = BaseAddress, ClassName = ServiceType.Name, MethodName = methodName, Arguments = arguments });
        }

        public virtual async Task<string> ReceiveGetResponseAsync(ServiceProxyInvokeRequest request)
        {
            ServiceProxyInvokeEventArgs args = request.CopyAs<ServiceProxyInvokeEventArgs>();
            args.Client = this;

            OnGetStarted(args);
            string result = string.Empty;
            if (args.CancelInvoke)
            {
                OnGetCanceled(args);
            }
            else
            {
                HttpRequestMessage requestMessage = await CreateServiceProxyRequestMessageAsync(ServiceProxyVerbs.Get, request.MethodName, request.Arguments);
                HttpResponseMessage response = await HttpClient.SendAsync(requestMessage);
                args.RequestMessage = requestMessage;
                args.ResponseMessage = response;
                result = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }
            return result;
        }

        protected internal async Task<HttpRequestMessage> CreateServiceProxyRequestMessageAsync(ServiceProxyVerbs verb, string methodName, params object[] arguments)
        {
            Dictionary<string, object> namedArguments = ApiArgumentProvider.GetNamedArguments(methodName, arguments);
            string queryString = ApiArgumentProvider.ArgumentsToQueryString(namedArguments);
            return await CreateServiceProxyRequestMessageAsync(verb, ApiArgumentProvider.ServiceType.Name, methodName, queryString);
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

        protected internal virtual void SetHttpArgsContent(string jsonArgsMemberString, HttpRequestMessage request)
        {
            request.Content = new StringContent(jsonArgsMemberString, Encoding.UTF8, JsonMediaType);
        }

        public abstract Task<string> InvokeServiceMethodAsync(string baseAddress, string className, string methodName, object[] arguments);

        public abstract Task<string> InvokeServiceMethodAsync(string className, string methodName, object[] arguments);

        public abstract Task<string> InvokeServiceMethodAsync(string methodName, object[] arguments);
    }
}
