/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net.Logging;
using Bam.Net.Web;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public interface IServiceProxyClient
    {
        IApiArgumentEncoder ApiArgumentEncoder { get; set; }
        string BaseAddress { get; set; }
        ILogger Logger { get; set; }
        Type ServiceType { get; }
        string UserAgent { get; set; }

        event EventHandler<ServiceProxyInvocationRequestEventArgs> GetCanceled;
        event EventHandler<ServiceProxyInvocationRequestEventArgs> GetComplete;
        event EventHandler<ServiceProxyInvocationRequestEventArgs> GetStarted;
        event EventHandler<ServiceProxyInvocationRequestEventArgs> InvocationException;
        event EventHandler<ServiceProxyInvocationRequestEventArgs> InvocationCanceled;
        event EventHandler<ServiceProxyInvocationRequestEventArgs> InvocationComplete;
        event EventHandler<ServiceProxyInvocationRequestEventArgs> InvocationStarted;
        event EventHandler<ServiceProxyInvocationRequestEventArgs> PostCanceled;
        event EventHandler<ServiceProxyInvocationRequestEventArgs> PostComplete;
        event EventHandler<ServiceProxyInvocationRequestEventArgs> PostStarted;

        Task<HttpRequestMessage> CreateServiceProxyInvocationRequestMessageAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest);

        HttpClientResponse InvokeServiceMethod(string methodName, params object[] parameters);
        Task<HttpClientResponse> InvokeServiceMethodAsync(string methodName, object[] arguments);
        Task<HttpClientResponse> InvokeServiceMethodAsync(string className, string methodName, object[] arguments);
        Task<HttpClientResponse> InvokeServiceMethodAsync(string baseAddress, string className, string methodName, object[] arguments);
        Task<HttpClientResponse> ReceiveGetResponseAsync(ServiceProxyInvocationRequest request);
        Task<HttpClientResponse> ReceiveGetResponseAsync(string methodName, params object[] arguments);
        Task<HttpClientResponse> ReceivePostResponseAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest);
        Task<HttpClientResponse> ReceivePostResponseAsync(string methodName, params object[] arguments);
    }
}