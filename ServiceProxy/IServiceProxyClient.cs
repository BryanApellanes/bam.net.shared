/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net.Logging;
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

        string InvokeServiceMethod(string methodName, params object[] parameters);
        Task<string> InvokeServiceMethodAsync(string methodName, object[] arguments);


        Task<string> InvokeServiceMethodAsync(string className, string methodName, object[] arguments);

        Task<string> InvokeServiceMethodAsync(string baseAddress, string className, string methodName, object[] arguments);
        Task<string> ReceiveGetResponseAsync(ServiceProxyInvocationRequest request);
        Task<string> ReceiveGetResponseAsync(string methodName, params object[] arguments);
        Task<string> ReceivePostResponseAsync(ServiceProxyInvocationRequest serviceProxyInvocationRequest);
        Task<string> ReceivePostResponseAsync(string methodName, params object[] arguments);
    }
}