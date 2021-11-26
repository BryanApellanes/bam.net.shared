using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bam.Net.Server.ServiceProxy
{
    public class ServiceProxyInvocationArgument
    {
        public ServiceProxyInvocationArgument(ServiceProxyInvocationArgumentReader argumentReader, ParameterInfo parameterInfo, string json)
        {
            this.ArgumentReader = argumentReader;
            this.ParameterInfo = parameterInfo;
            this.Json = json;            
        }

        protected ServiceProxyInvocationArgumentReader ArgumentReader
        {
            get;
        }


        public string Json 
        {
            get;
        }

        public object Value 
        {
            get
            {
                Args.ThrowIfNull(ParameterInfo, nameof(ParameterInfo));
                return ArgumentReader.DecodeValue(ParameterInfo.ParameterType, Json);
            }
        }

        public string MethodName
        {
            get
            {
                Args.ThrowIfNull(ParameterInfo, nameof(ParameterInfo));
                return ParameterInfo.Member?.Name;
            }
        }

        public string Name
        {
            get
            {
                Args.ThrowIfNull(ParameterInfo, nameof(ParameterInfo));
                return ParameterInfo.Name;
            }
        }

        public ParameterInfo ParameterInfo { get; set; }
    }
}
