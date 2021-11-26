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
            this.MethodUrlFormat = "{BaseAddress}serviceproxy/{ClassName}/{MethodName}?{Arguments}";
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

        public event EventHandler<ServiceProxyInvocationRequestEventArgs> RequestExceptionThrown;

        protected void OnRequestExceptionThrown(ServiceProxyInvocationRequestEventArgs args)
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

            OnInvokeMethodCanceled(args);
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
        protected void OnPosting(ServiceProxyInvocationRequestEventArgs args)
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

            OnInvokeMethodCanceled(args);
        }

        /// <summary>
        /// Fires the Got event
        /// </summary>
        /// <param name="args"></param>
        protected void OnPosted(ServiceProxyInvocationRequestEventArgs args)
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
        public event EventHandler<ServiceProxyInvocationRequestEventArgs> InvocationException;

        protected void OnInvocationException(ServiceProxyInvocationRequestEventArgs args)
        {
            InvocationException?.Invoke(this, args);
        }

        public event EventHandler<ServiceProxyInvocationRequestEventArgs> InvokeMethodStarted;
        protected void OnInvokeMethodStarted(ServiceProxyInvocationRequestEventArgs args)
        {
            InvokeMethodStarted?.Invoke(this, args);
        }

        public event EventHandler<ServiceProxyInvocationRequestEventArgs> InvokeMethodComplete;
        protected void OnInvokeMethodComplete(ServiceProxyInvocationRequestEventArgs args)
        {
            InvokeMethodComplete?.Invoke(this, args);
        }

        public event EventHandler<ServiceProxyInvocationRequestEventArgs> InvokeMethodCanceled;
        protected void OnInvokeMethodCanceled(ServiceProxyInvocationRequestEventArgs args)
        {
            InvokeMethodCanceled?.Invoke(this, args);
        }

        public async Task<string> ReceivePostResponseAsync(string methodName, params object[] arguments)
        {
            return await ReceivePostResponseAsync(new ServiceProxyInvocationRequest(this, BaseAddress, ServiceType.Name, methodName, arguments));
        }

        public virtual async Task<string> ReceivePostResponseAsync(ServiceProxyInvocationRequest serviceProxyInvokeRequest)
        {
            ServiceProxyInvocationRequestEventArgs args = new ServiceProxyInvocationRequestEventArgs(serviceProxyInvokeRequest);
            args.Client = this;

            OnPosting(args);
            string result = string.Empty;
            if (args.CancelInvoke)
            {
                OnPostCanceled(args);
            }
            else
            {
                ServiceProxyInvocationRequestArguments serviceProxyArguments = serviceProxyInvokeRequest.ServiceProxyInvocationRequestArguments;
                HttpRequestMessage request = await CreateServiceProxyRequestMessageAsync(serviceProxyInvokeRequest.Verb, serviceProxyInvokeRequest.ClassName, serviceProxyInvokeRequest.MethodName, "nocache=".RandomLetters(4));
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
            return await ReceiveGetResponseAsync(new ServiceProxyInvocationRequest(this, BaseAddress, ServiceType.Name, methodName, arguments));
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

        protected internal async Task<HttpRequestMessage> CreateServiceProxyRequestMessageWithArgumentsAsync(ServiceProxyVerbs verb, string methodName, params object[] arguments)
        {
            Dictionary<string, object> namedArguments = ApiArgumentProvider.GetNamedArguments(methodName, arguments);
            string queryString = ApiArgumentProvider.ArgumentsToQueryString(namedArguments);
            return await CreateServiceProxyRequestMessageAsync(verb, ApiArgumentProvider.ServiceType.Name, methodName, queryString);
        }
        
        protected virtual internal Task<HttpRequestMessage> CreateServiceProxyRequestMessageAsync(ServiceProxyVerbs verb, string className, string methodName, string queryStringArguments = "")
        {
            string methodUrl = MethodUrlFormat.NamedFormat(new { BaseAddress, ClassName = className, MethodName = methodName, Arguments = queryStringArguments });
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

        public string GetServiceProxyInvocationUrl(ServiceProxyInvocationRequest serviceProxyInvocationRequest, ServiceProxyInvocationRequestArguments serviceProxyInvocationRequestArguments)
        {
            return GetServiceProxyInvocationUrl(serviceProxyInvocationRequest.ClassName, serviceProxyInvocationRequest.MethodName, serviceProxyInvocationRequestArguments);
        }

        public string GetServiceProxyInvocationUrl(string className, string methodName, ServiceProxyInvocationRequestArguments serviceProxyInvocationRequestArguments)
        {
            return MethodUrlFormat.NamedFormat(new 
            {
                BaseAddress,
                ClassName = className,
                MethodName = methodName,
                Arguments = serviceProxyInvocationRequestArguments.QueryStringArguments
            });
        }

        public abstract Task<string> InvokeServiceMethodAsync(string baseAddress, string className, string methodName, object[] arguments);

        public abstract Task<string> InvokeServiceMethodAsync(string className, string methodName, object[] arguments);

        public abstract Task<string> InvokeServiceMethodAsync(string methodName, object[] arguments);
    }
}
