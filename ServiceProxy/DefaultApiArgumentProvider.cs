using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Bam.Net.ServiceProxy
{

    public class DefaultApiArgumentProvider : IApiArgumentProvider
    {
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

        public string ArgumentsToJsonArgsMember(params object[] arguments)
        {
            return ApiArguments.ArgumentsToJsonArgsMember(arguments);
        }

        public string GetStringToHash(ServiceProxyInvocation request)
        {
            return ApiArguments.GetStringToHash(request);
        }

        public string GetStringToHash(string className, string methodName, string jsonArguments)
        {
            return ApiArguments.GetStringToHash(className, methodName, jsonArguments);
        }

        public string GetArgumentFromValue(object value)
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
                 return WebUtility.UrlEncode(value.ToJson());
             }
        }

        public Dictionary<string, object> GetNamedArguments(string methodName, object[] parameters)
        {
            if (!Methods.Contains(methodName))
            {
                throw Args.Exception<InvalidOperationException>("{0} is not proxied from type {1}", methodName, ServiceType.Name);
            }

            MethodInfo method = ServiceType.GetMethod(methodName, parameters.Select(obj => obj.GetType()).ToArray());

            Dictionary<string, object> result = GetNamedArguments(method, parameters);

            return result;
        }

        public Dictionary<string, object> GetNamedArguments(MethodInfo method, object[] parameters)
        {
            List<ParameterInfo> parameterInfos = new List<ParameterInfo>(method.GetParameters());
            parameterInfos.Sort((l, r) => l.MetadataToken.CompareTo(r.MetadataToken));

            if (parameters.Length != parameterInfos.Count)
            {
                throw new InvalidOperationException("Parameter count mismatch");
            }

            Dictionary<string, object> result = new Dictionary<string, object>();
            parameterInfos.Each((pi, i) =>
            {
                result[pi.Name] = parameters[i];
            });
            return result;
        }

        public string ArgumentsToNumberedQueryString(object[] arguments)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < arguments.Length; i++)
            {
                if (i != 0)
                {
                    result.Append("&");
                }

                result.AppendFormat("{0}={1}", i, GetArgumentFromValue(arguments[i]));
            }

            return result.ToString();
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

                result.AppendFormat("{0}={1}", key, GetArgumentFromValue(arguments[key]));
                first = false;
            }

            return result.ToString();
        }

        public string[] ArgumentsToJsonArgumentsArray(params object[] arguments)
        {
            return ApiArguments.ArgumentsToJsonArgumentsArray(arguments);
        }

        static DefaultApiArgumentProvider _current;
        static object _currentLock = new object();
        public static DefaultApiArgumentProvider Current
        {
            get
            {
                return _currentLock.DoubleCheckLock(ref _current, () => new DefaultApiArgumentProvider());
            }
        }
    }
}
