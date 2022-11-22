/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Bam.Net.Logging;
using Bam.Net.Encryption;
using Bam.Net.Configuration;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System.IO;
using System.Reflection;
using System.Collections;
using Bam.Net;
using Newtonsoft.Json;
using Bam.Net.Server.ServiceProxy;
using System.Web;

namespace Bam.Net.ServiceProxy
{
    /// <summary>
    /// A class used to properly format parameters for service proxy calls.
    /// </summary>
    public class ApiArgumentEncoder : IApiArgumentEncoder
    {
        public ApiArgumentEncoder()
        {
            this.ValueEncoder = new JsonTransformer<object>();
        }

        public IValueTransformer<object, string> ValueEncoder { get; private set; }
        public Type ServiceType { get; set; }
        HashSet<string> _methods;
        object _methodsLock = new object();
        public HashSet<string> Methods
        {
            get
            {
                return _methodsLock.DoubleCheckLock(ref _methods, () => new HashSet<string>(ServiceProxySystem.GetProxiedMethods(ServiceType).Select(m => m.Name).ToArray()));
            }
        }

        /// <summary>
        /// Gets the string that is validated for the specified request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetValidationString(ServiceProxyInvocationRequest request)
        {
            return GetValidationString(request.ClassName, request.MethodName, GetArgumentsQueryString(request.ServiceProxyInvocationRequestArgumentWriter));
        }

        /// <summary>
        /// Gets the string that is validated for the specified request.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="jsonArguments"></param>
        /// <returns></returns>
        public string GetValidationString(string className, string methodName, string jsonArguments)
        {
            return string.Format("{0}.{1}.{2}", className, methodName, jsonArguments);
        }

        public string EncodeValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            Type type = value.GetType();
            if (type == typeof(string) ||
                type == typeof(int) ||
                type == typeof(decimal) ||
                type == typeof(long))
            {
                return value.ToString();
            }
            else
            {
                return HttpUtility.UrlEncode(value.ToJson());
            }
        }

        public string GetArgumentsQueryString(ServiceProxyInvocationRequestArgumentWriter requestArguments)
        {
            return ArgumentsToQueryString(requestArguments.NamedArguments);
        }

        public Dictionary<string, object> GetNamedArguments(string methodName, object[] arguments)
        {
            if (!Methods.Contains(methodName))
            {
                throw Args.Exception<InvalidOperationException>("{0} is not proxied from type {1}", methodName, ServiceType.Name);
            }

            MethodInfo method = ServiceType.GetMethod(methodName, arguments.Select(obj => obj.GetType()).ToArray());

            Dictionary<string, object> result = GetNamedArguments(method, arguments);

            return result;
        }

        public Dictionary<string, object> GetNamedArguments(MethodInfo method, object[] arguments)
        {
            List<ParameterInfo> parameterInfos = new List<ParameterInfo>(method.GetParameters());
            parameterInfos.Sort((l, r) => l.MetadataToken.CompareTo(r.MetadataToken));

            if (arguments.Length != parameterInfos.Count)
            {
                throw new InvalidOperationException("Argument count mismatch");
            }

            Dictionary<string, object> result = new Dictionary<string, object>();
            parameterInfos.Each((pi, i) =>
            {
                result[pi.Name] = arguments[i];
            });
            return result;
        }

        public string ArgumentsToQueryString(Dictionary<string, object> arguments)
        {
            StringBuilder result = new StringBuilder();
            bool first = true;
            foreach (string key in arguments.Keys)
            {
                if (!first)
                {
                    result.Append("&");
                }

                result.AppendFormat("{0}={1}", key, EncodeValue(arguments[key]));
                first = false;
            }

            return result.ToString();
        }

        public string[] ArgumentsToJsonArgumentsArray(params object[] arguments)
        {
            // create a string array
            string[] jsonArgs = new string[arguments.Length];

            JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            // for each parameter stringify it and put it into the array
            arguments.Each((o, i) => jsonArgs[i] = o.ToJson(settings));
            return jsonArgs;
        }

        public string ArgumentsToJsonArgsMember(params object[] arguments)
        {
            string[] jsonArguments = ArgumentsToJsonArgumentsArray(arguments);
            string jsonArgumentsString = (new
            {
                jsonArgs = jsonArguments.ToJson()
            }).ToJson();

            return jsonArgumentsString;
        }

    }
}
