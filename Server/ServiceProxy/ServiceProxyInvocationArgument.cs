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

        public ServiceProxyInvocationArgument(ServiceProxyInvocationArgumentReader argumentReader, ParameterInfo parameterInfo, object value)
        {
            this.ArgumentReader = argumentReader;
            this.ParameterInfo = parameterInfo;
            this.Json = value.ToJson();
            this._value = value;
        }

        protected ServiceProxyInvocationArgumentReader ArgumentReader
        {
            get;
        }


        public string Json 
        {
            get;
        }

        object _value;
        public object Value 
        {
            get
            {
                if (_value == null)
                {
                    Args.ThrowIfNull(ParameterInfo, nameof(ParameterInfo));
                    _value = ArgumentReader.DecodeValue(ParameterInfo.ParameterType, Json);
                }
                return _value;
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

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
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
