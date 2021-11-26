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
        public ServiceProxyClient(HttpClient httpClient) : base(httpClient, typeof(TService))
        {
        }

        public ServiceProxyClient(string baseAddress)
            : base(typeof(TService), baseAddress)
        {
            if (!string.IsNullOrEmpty(BaseAddress) && !BaseAddress.EndsWith("/"))
            {
                BaseAddress = string.Format("{0}/", BaseAddress);
            }
        }

        public async Task<TResult> InvokeServiceMethodAsync<TResult>(string methodName, params object[] arguments)
        {
            return await Task.Run(() => InvokeServiceMethod<TResult>(methodName, arguments));
        }

        /// <summary>
        /// Invoke the specified remote service method.
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
            return Task.Run(() => InvokeServiceMethodAsync(BaseAddress, typeof(TService).Name, methodName, arguments));
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
            return InvokeServiceMethodAsync(BaseAddress, typeof(TService).Name, methodName, parameters).Result;
        }

        public override Task<string> InvokeServiceMethodAsync(string className, string methodName, object[] arguments)
        {
            return InvokeServiceMethodAsync(BaseAddress, className, methodName, arguments);
        }

        public override async Task<string> InvokeServiceMethodAsync(string baseAddress, string className, string methodName, params object[] arguments)
        {
            ServiceProxyInvocationRequest<TService> request = new ServiceProxyInvocationRequest<TService>(this, baseAddress, className, methodName, arguments);

            ServiceProxyInvocationRequestEventArgs args = request.CopyAs<ServiceProxyInvocationRequestEventArgs<TService>>(request);
            OnInvokeMethodStarted(args);
            string response = string.Empty;
            if (args.CancelInvoke)
            {
                OnInvokeMethodCanceled(args);
            }
            else
            {
                response = await ReceiveServiceMethodResponseAsync(request);
                LastResponse = response;
                OnInvokeMethodComplete(args);
            }

            return response;
        }

        /// <summary>
        /// Sends the specified request and returns the response body.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected internal virtual async Task<string> ReceiveServiceMethodResponseAsync(ServiceProxyInvocationRequest<TService> request)
        {
            string result = string.Empty;
            try
            {
                return await request.ExecuteAsync(this);
            }
            catch (Exception ex)
            {
                ServiceProxyInvocationRequestEventArgs<TService> args = request.CopyAs<ServiceProxyInvocationRequestEventArgs<TService>>();
                args.Exception = ex;
                OnInvocationException(args);
            }
            return result;
        }

    }
}
