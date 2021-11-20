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

        public event EventHandler<ServiceProxyInvokeEventArgs> RequestExceptionThrown;

        protected void OnRequestExceptionThrown(ServiceProxyInvokeEventArgs args)
        {
            if (RequestExceptionThrown != null)
            {
                RequestExceptionThrown(this, args);
            }
        }

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
        protected void OnGetComplete(ServiceProxyInvokeEventArgs args)
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
                return _apiArgumentProviderLock.DoubleCheckLock(ref _apiArgumentProvider, () => new DefaultApiArgumentProvider() { ServiceType = ServiceType });
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
/*
        /// <summary>
        /// The class responsible for providing the name of the
        /// current application.
        /// </summary>
        public IApplicationNameProvider ClientApplicationNameProvider { get; set; }*/

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
            return await ReceivePostResponseAsync(new ServiceProxyInvokeRequest(this, BaseAddress, ServiceType.Name, methodName, arguments));
        }

        public virtual async Task<string> ReceivePostResponseAsync(ServiceProxyInvokeRequest serviceProxyInvokeRequest)
        {
            ServiceProxyInvokeEventArgs args = new ServiceProxyInvokeEventArgs(serviceProxyInvokeRequest);
            args.Client = this;

            OnPosting(args);
            string result = string.Empty;
            if (args.CancelInvoke)
            {
                OnPostCanceled(args);
            }
            else
            {
                ServiceProxyArguments serviceProxyArguments = serviceProxyInvokeRequest.ServiceProxyArguments;
                HttpRequestMessage request = await CreateServiceProxyRequestMessageAsync(serviceProxyArguments.Verb, serviceProxyInvokeRequest.ClassName, serviceProxyInvokeRequest.MethodName, "nocache=".RandomLetters(4));
                serviceProxyArguments.SetContent(request);

                try
                {
                    HttpResponseMessage response = await HttpClient.SendAsync(request);
                    args.RequestMessage = request;
                    args.ResponseMessage = response;
                    result = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    OnPosted(args);
                }
                catch (Exception ex)
                {
                    args.Exception = ex;
                    OnRequestExceptionThrown(args);
                }
            }
            return result;
        }

        public async Task<string> ReceiveGetResponseAsync(string methodName, params object[] arguments)
        {
            MethodInfo methodInfo = ServiceType.GetMethod(methodName, arguments.Select(argument => argument.GetType()).ToArray());
            return await ReceiveGetResponseAsync(new ServiceProxyInvokeRequest(this, BaseAddress, ServiceType.Name, methodName, arguments));
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
                HttpRequestMessage requestMessage = await CreateServiceProxyRequestMessageWithArgumentsAsync(ServiceProxyVerbs.Get, request.MethodName, request.Arguments);
                HttpResponseMessage response = await HttpClient.SendAsync(requestMessage);
                args.RequestMessage = requestMessage;
                args.ResponseMessage = response;
                result = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                OnGetComplete(args);
            }
            return result;
        }

/*        protected virtual internal Task<HttpRequestMessage> CreateServiceProxyRequestMessageAsync(ServiceProxyInvokeRequest request, ServiceProxyArguments arguments)
        {
            return CreateServiceProxyRequestMessageAsync(arguments.Verb, request.ClassName, request.MethodName, arguments.QueryStringArguments);
        }*/

        protected internal async Task<HttpRequestMessage> CreateServiceProxyRequestMessageWithArgumentsAsync(ServiceProxyVerbs verb, string methodName, params object[] arguments)
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

        public abstract Task<string> InvokeServiceMethodAsync(string baseAddress, string className, string methodName, object[] arguments);

        public abstract Task<string> InvokeServiceMethodAsync(string className, string methodName, object[] arguments);

        public abstract Task<string> InvokeServiceMethodAsync(string methodName, object[] arguments);
    }
}
