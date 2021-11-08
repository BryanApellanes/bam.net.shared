using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvokeRequest<TService> : ServiceProxyInvokeRequest
    {
        public ServiceProxyInvokeRequest()
        {
            this.Type = typeof(TService);
        }

        public new ServiceProxyClient<TService> Client
        {
            get;
            set;
        }

        public Type Type { get; set; }

        string _className;
        public override string ClassName 
        {
            get
            {
                if(string.IsNullOrEmpty(_className))
                {
                    _className = Type?.Name;
                }
                return _className;
            }
            set
            {
                _className = value;
            }
        }

        string _queryStringArguments;
        object _queryStringArgumentsLock = new object();
        public override string QueryStringArguments
        {
            get 
            {
                return _queryStringArgumentsLock.DoubleCheckLock(ref _queryStringArguments, () => new ServiceProxyArguments<TService>(MethodName, Arguments).QueryStringArguments);
            }
            set 
            {
                _queryStringArguments = value;
            }
        }

        ServiceProxyArguments<TService> _serviceProxyArguments;
        public ServiceProxyArguments<TService> ServiceProxyArguments
        {
            get
            {
                if (_serviceProxyArguments == null)
                {
                    _serviceProxyArguments = new ServiceProxyArguments<TService>(this);
                }
                return _serviceProxyArguments;                
            }
        }

        public async Task<TResult> ExecuteAsync<TResult>(ServiceProxyClient<TService> client)
        {
            string response = await ExecuteAsync(client);
            return response.FromJson<TResult>();
        }

        public async Task<string> ExecuteAsync(ServiceProxyClient<TService> client)
        {
            this.Client = client;
            if (ServiceProxyArguments.Verb == ServiceProxyVerbs.Post)
            {
                return await client.ReceivePostResponseAsync(this);
            }
            else
            {
                return await client.ReceiveGetResponseAsync(this);
            }
        }
    }
}
