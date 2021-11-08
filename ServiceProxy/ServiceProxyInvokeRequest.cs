using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvokeRequest
    {
        public ServiceProxyInvokeRequest()
        {
            Cuid = NCuid.Cuid.Generate();
        }

        public virtual ServiceProxyClient Client { get; set; }
        public string Cuid { get; internal set; }

        public bool CancelInvoke { get; set; }
        public string BaseAddress { get; set; }
        public virtual string ClassName { get; set; }
        public string MethodName { get; set; }
        public virtual string QueryStringArguments { get; set; }
        public object[] Arguments { get; set; }

        public async Task<TResult> ExecuteAsync<TService, TResult>(ServiceProxyClient client)
        {
            this.Client = client;
            return await client.ReceiveServiceMethodResponseAsync<TService, TResult>(MethodName, Arguments);
        }
    }
}
