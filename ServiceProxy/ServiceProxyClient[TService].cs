/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyClient<TService> : ServiceProxyClient
    {
        public ServiceProxyClient(string baseAddress)
            : base(baseAddress)
        {
            if (!string.IsNullOrEmpty(BaseAddress) && !BaseAddress.EndsWith("/"))
            {
                BaseAddress = string.Format("{0}/", BaseAddress);
            }
        }

        public ServiceProxyClient(string baseAddress, string implementingClassName)
            : this(baseAddress)
        {
            this.ClassName = implementingClassName;
        }

        IApiArgumentProvider<TService> _apiArgumentProvider;
        object _apiArgumentProviderLock = new object();
        public new IApiArgumentProvider<TService> ApiArgumentProvider
        {
            get
            {
                return _apiArgumentProviderLock.DoubleCheckLock(ref _apiArgumentProvider, () => new DefaultApiArgumentProvider<TService>());
            }
            set
            {
                _apiArgumentProvider = value;
            }
        }

        string _className;
        object _classNameLock = new object();
        /// <summary>
        /// Gets or sets the name of the implementing class on the server.  If typeof(T)
        /// is an interface as determined by typeof(T).IsInterface then it
        /// is assumed that the classname equals typeof(T).Name.Substring(1)
        /// which drops the first character of the name.
        /// </summary>
        public string ClassName
        {
            get
            {
                return _classNameLock.DoubleCheckLock(ref _className, () => typeof(TService).IsInterface ? typeof(TService).Name.Substring(1) : typeof(TService).Name);
            }
            set
            {
                _className = value;
            }
        }

        HashSet<string> _methods;
        object _methodsLock = new object();
        public HashSet<string> Methods
        {
            get
            {
                return _methodsLock.DoubleCheckLock(ref _methods, () => new HashSet<string>(ServiceProxySystem.GetProxiedMethods(typeof(TService)).Select(m => m.Name).ToArray()));
            }
        }

        public string LastResponse { get; private set; }

        public async Task<TResult> InvokeServcieMethodAsync<TResult>(string methodName, params object[] arguments)
        {
            return await Task.Run(() => InvokeServiceMethod<TResult>(methodName, arguments));
        }

        /// <summary>
        /// Invoke the specified methodName on the server side
        /// type T returning value of type T1
        /// </summary>
        /// <typeparam name="TResult">The return type of the specified method</typeparam>
        /// <param name="methodName">The name of the method to invoke</param>
        /// <param name="arguments">parameters to be passed to the method</param>
        /// <returns></returns>
        public TResult InvokeServiceMethod<TResult>(string methodName, params object[] arguments)
        {
            string result = InvokeServiceMethod(methodName, arguments);
            return result.FromJson<TResult>();
        }

        public override Task<string> InvokeServiceMethodAsync(string methodName, object[] arguments)
        {
            return Task.Run(() => InvokeServiceMethodAsync(BaseAddress, ClassName, methodName, arguments));
        }

        /// <summary>
        /// Invoke the specified methodName on the server side
        /// type T
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string InvokeServiceMethod(string methodName, params object[] parameters)
        {
            return InvokeServiceMethodAsync(BaseAddress, ClassName, methodName, parameters).Result;
        }

        public override Task<string> InvokeServiceMethodAsync(string className, string methodName, object[] arguments)
        {
            return InvokeServiceMethodAsync(BaseAddress, className, methodName, arguments);
        }

        public override async Task<string> InvokeServiceMethodAsync(string baseAddress, string className, string methodName, params object[] arguments)
        {
            if (!Methods.Contains(methodName) && typeof(TService).Name.Equals(className))
            {
                throw Args.Exception<InvalidOperationException>("{0} is not proxied from type {1}", methodName, className);
            }
            ServiceProxyInvokeRequest<TService> request = new ServiceProxyInvokeRequest<TService>() { BaseAddress = baseAddress, ClassName = className, Client = this, MethodName = methodName, Arguments = arguments };

            ServiceProxyInvokeEventArgs args = request.CopyAs<ServiceProxyInvokeEventArgs>();
            OnInvokeMethodStarted(args);
            string result = string.Empty;
            if(args.CancelInvoke)
            {
                OnInvokeMethodCanceled(args);
            }
            else
            {
                result = await ReceiveServiceMethodResponseAsync(request);
                LastResponse = result;
                OnInvokeMethodComplete(args);
            }

            return result;
        }

        /// <summary>
        /// Sends the specified request and returns the response body.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected internal virtual async Task<string> ReceiveServiceMethodResponseAsync(ServiceProxyInvokeRequest<TService> request)
        {
            string result = string.Empty;
            try
            {
                return await request.ExecuteAsync(this);
            }
            catch (Exception ex)
            {
                ServiceProxyInvokeEventArgs<TService> args = request.CopyAs<ServiceProxyInvokeEventArgs<TService>>();
                args.Exception = ex;
                OnInvocationException(args);
            }
            return result;
        }

        protected internal async Task<HttpRequestMessage> CreateServiceProxyRequestMessageAsync(ServiceProxyVerbs verb, string methodName, params object[] arguments)
        {
            Dictionary<string, object> namedArguments = ApiArgumentProvider.GetNamedArguments(methodName, arguments);
            string queryString = ApiArgumentProvider.ArgumentsToQueryString(namedArguments);
            return await base.CreateServiceProxyRequestMessageAsync(verb, typeof(TService).Name, methodName, queryString);
        }       

        public async Task<string> ReceiveGetResponseAsync(string methodName, params object[] arguments)
        {
            return await ReceiveGetResponseAsync(new ServiceProxyInvokeRequest { BaseAddress = BaseAddress, ClassName = typeof(TService).Name, MethodName = methodName, Arguments = arguments, QueryStringArguments = new ServiceProxyArguments<TService>(methodName, arguments).NumberedQueryStringParameters });
        }

        public virtual async Task<string> ReceiveGetResponseAsync(ServiceProxyInvokeRequest request)
        {
            ServiceProxyInvokeEventArgs<TService> args = request.CopyAs<ServiceProxyInvokeEventArgs<TService>>();
            args.Client = this;
            
            OnGetStarted(args);
            string result = string.Empty;
            if(args.CancelInvoke)
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

        public async Task<string> ReceivePostResponseAsync(string methodName, params object[] arguments)
        {
            return await ReceivePostResponseAsync(new ServiceProxyInvokeRequest<TService>() { BaseAddress = BaseAddress, MethodName = methodName, Arguments = arguments });
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
            if(args.CancelInvoke)
            {
                OnPostCanceled(args);
            }
            else
            {
                string jsonArguments = ApiArgumentProvider.ArgumentsToJsonArgumentsObjectString(args.Arguments);
                Uri uri = new Uri(args.BaseAddress);
                if(HttpClient.BaseAddress == null || !HttpClient.BaseAddress.ToString().Equals(uri.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    HttpClient.BaseAddress = uri;
                }
                SetHttpArgsContent(jsonArguments, request);
                HttpResponseMessage response = await HttpClient.SendAsync(request);
                args.RequestMessage = request;
                args.ResponseMessage = response;
                result = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                OnPosted(args);
            }
            return result;
        }

        protected internal virtual void SetHttpArgsContent(string jsonArgsString, HttpRequestMessage request)
        {
            request.Content = new StringContent(jsonArgsString, Encoding.UTF8, JsonMediaType);
        }
    }
}
