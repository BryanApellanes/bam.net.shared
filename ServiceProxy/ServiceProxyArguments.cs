/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Bam.Net.ServiceProxy
{
    /// <summary>
    /// Encapsulates method and parameters for 
    /// a ServiceProxy call.
    /// </summary>
    public class ServiceProxyArguments
    {
        public ServiceProxyArguments(object[] arguments)
        {
            this.Arguments = arguments;
            this.ApiArgumentProvider = DefaultApiArgumentProvider.Current;
        }

        public ServiceProxyArguments(MethodInfo method, object[] arguments)
        {
            this.Method = method;
            this.Arguments = arguments;
            this.ApiArgumentProvider = DefaultApiArgumentProvider.Current;
        }

        public virtual IApiArgumentProvider ApiArgumentProvider
        {
            get;
            set;
        }

        string _methodName;
        public string MethodName
        {
            get
            {
                if (string.IsNullOrEmpty(_methodName) && Method != null)
                {
                    _methodName = Method.Name;
                }
                return _methodName;
            }
            set
            {
                _methodName = value;
            }
        }


        public MethodInfo Method
        {
            get;
            set;
        }

        public object[] Arguments
        {
            get;
            set;
        }

        public string NumberedQueryStringParameters
        {
            get
            {
                return ApiArgumentProvider.ArgumentsToNumberedQueryString(Arguments);
            }
        }

        public virtual ServiceProxyVerbs Verb
        {
            get
            {
                return NumberedQueryStringParameters?.Length > 2048 ? ServiceProxyVerbs.Post : ServiceProxyVerbs.Get;
            }
        }
    }
}
