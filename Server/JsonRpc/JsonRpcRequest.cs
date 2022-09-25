using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Net.ServiceProxy;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server.JsonRpc
{
    public class JsonRpcRequest: JsonRpcNotification, IJsonRpcRequest
    {
        public object Id { get; set; }

        /// <summary>
        /// Execute the request and return the response
        /// </summary>
        /// <returns></returns>
        public override JsonRpcResponse Execute()
        {
            JsonRpcResponse response = base.Execute();
            response.Id = Id;
            return response;
        }

        public new static JsonRpcRequest Parse(string json)
        {
            return json.FromJson<JsonRpcRequest>();
        }

        public new static JsonRpcRequest Create<T>(WebServiceRegistry incubator, string methodName, params object[] parameters)
        {
            return Create<T>(incubator, (object)Guid.NewGuid().ToString(), methodName, parameters);            
        }

        public new static JsonRpcRequest Create<T>(string methodName, params object[] parameters)
        {
            return Create<T>((object)Guid.NewGuid().ToString(), methodName, parameters);
        }

        public static JsonRpcRequest Create<T>(object id, string methodName, params object[] parameters)
        {
            return Create<T>(new WebServiceRegistry(), id, methodName, parameters);
        }

        public static JsonRpcRequest Create<T>(WebServiceRegistry incubator, object id, string methodName, params object[] parameters)
        {
            return Create(incubator, id, typeof(T).GetMethod(methodName, parameters.Select(p => p.GetType()).ToArray()), parameters);
        }

        public new static JsonRpcRequest Create(MethodInfo method, params object[] parameters)
        {
            return Create(Guid.NewGuid().ToString(), method, parameters);
        }

        public static JsonRpcRequest Create(object id, MethodInfo method, params object[] parameters)
        {
            return Create(new WebServiceRegistry(), id, method, parameters);
        }

        public new static JsonRpcRequest Create(WebServiceRegistry incubator, MethodInfo method, params object[] parameters)
        {
            return Create(incubator, (object)Guid.NewGuid().ToString(), method, parameters);            
        }

        public static JsonRpcRequest Create(WebServiceRegistry incubator, object id, MethodInfo method, params object[] parameters)
        {
            JsonRpcNotification notification = JsonRpcNotification.Create(incubator, method, parameters);
            JsonRpcRequest request = notification.CopyAs<JsonRpcRequest>();
            request.ServiceRegistry = incubator;
            request.Id = id;
            return request;
        }
    }
}
