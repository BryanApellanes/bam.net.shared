using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Net.ServiceProxy;
using Bam.Net.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server.JsonRpc
{
    public partial class JsonRpcNotification : JsonRpcMessage, IJsonRpcRequest
    {
        public JsonRpcNotification()
        {
            this.RpcParams = new JsonRpcParameters();
        }
        
        [Exclude]
        public object Clone()
        {
            JsonRpcNotification clone = new JsonRpcNotification();
            clone.CopyProperties(this);
            return clone;
        }

        public IHttpContext HttpContext { get; set; }

        public string Method { get; set; }

        WebServiceRegistry _serviceRegistry;
        protected internal WebServiceRegistry ServiceRegistry
        {
            get
            {
                return _serviceRegistry;
            }
            set
            {
                _serviceRegistry = value;
            }
        }

        Type[] _types;
        object _typesLock = new object();
        protected internal Type[] RpcTypes
        {
            get
            {
                return _typesLock.DoubleCheckLock(ref _types, () => RpcMethods.Select(m => m.DeclaringType).ToArray());
            }
        }

        MethodInfo[] _methods;
        object _methodsLock = new object();
        protected internal MethodInfo[] RpcMethods
        {
            get
            {
                return _methodsLock.DoubleCheckLock(ref _methods, () => ServiceRegistry.ClassNameTypes.SelectMany(type => type.GetMethodsWithAttributeOfType<JsonRpcMethodAttribute>()).ToArray());
            }
        }

        protected internal MethodInfo[] AllMethods
        {
            get
            {
                return ServiceRegistry.ClassNameTypes.SelectMany(type => type.GetMethods()).ToArray();
            }
        }

        /// <summary>
        /// The parameters.  This object will be either
        /// a JObject or JArray as indicated by the state
        /// of RpcParams
        /// </summary>
        public JToken Params { get; set; }
        public JsonRpcParameters RpcParams { get; set; }

        protected virtual JsonRpcResponse GetErrorResponse(JsonRpcFaultCodes faultCode)
        {
            JsonRpcResponse response = new JsonRpcResponse();
            JsonRpcFaultCode code = JsonRpcFaults.ByCode[(long)faultCode];
            JsonRpcError error = new JsonRpcError();
            error.Code = code.Code;
            error.Message = code.Message;
            error.Data = code.Meaning;
            response.Error = error;

            return response;
        }

        private object[] GetInputParameters(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            List<object> results = new List<object>(parameters.Length);
            if (RpcParams.Ordered)
            {
                ((JArray)Params).Each((jToken, i) =>
                {
                    ParameterInfo parameter = parameters[i];
                    AddParameter(results, jToken, parameter);
                });
            }
            else if (RpcParams.Named)
            {
                JObject parameterJson = (JObject)Params;
                parameters.Each(parameter =>
                {
                    JToken token = parameterJson[parameter.Name];
                    AddParameter(results, token, parameter);
                });
            }
            else
            {
                throw new InvalidOperationException("RpcParams is neither Named nor Ordered, it must be one or the other");
            }
            return results.ToArray();
        }

        public static JsonRpcNotification Create<T>(WebServiceRegistry serviceProvider, string methodName, params object[] parameters)
        {
            JsonRpcNotification result = Create<T>(methodName, parameters);
            result.ServiceRegistry = serviceProvider;
            return result;
        }

        public static JsonRpcNotification Create(MethodInfo method, params object[] parameters)
        {
            return Create(new WebServiceRegistry(), method, parameters);
        }

        public static JsonRpcNotification Create<T>(string methodName, params object[] parameters)
        {
            return Create(typeof(T).GetMethod(methodName, parameters.Select(p => p.GetType()).ToArray()), parameters);
        }

        public static JsonRpcNotification Create(WebServiceRegistry serviceProvider, MethodInfo method, params object[] parameters)
        {
            JsonRpcNotification result = new JsonRpcNotification();
            result.ServiceRegistry = serviceProvider;
            result.Method = method.Name;
            result.RpcParams.By.Position = parameters;
            result.Params = JToken.Parse(parameters.ToJson());
            return result;
        }

        private static void AddParameter(List<object> results, JToken jToken, ParameterInfo parameter)
        {
            if (jToken.HasValues)
            {
                object paramObject = JsonConvert.DeserializeObject(jToken.ToString(), parameter.ParameterType);
                results.Add(paramObject);
            }
            else
            {
                results.Add(Convert.ChangeType(jToken, parameter.ParameterType));
            }
        }
    }
}
