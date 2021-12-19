using Bam.Net.Server.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public interface IApiArgumentEncoder
    {
        string GetStringToHash(ServiceProxyInvocationRequest request);
        string GetStringToHash(string className, string methodName, string jsonArguments);

        string ArgumentsToJsonArgsMember(params object[] arguments);

        string[] ArgumentsToJsonArgumentsArray(params object[] arguments);

        IValueEncoder<object, string> ValueEncoder { get; }

        Type ServiceType { get; }

        HashSet<string> Methods { get; }

        string GetArgumentsQueryString(ServiceProxyInvocationRequestArguments arguments);

        Dictionary<string, object> GetNamedArguments(MethodInfo method, object[] arguments);
        Dictionary<string, object> GetNamedArguments(string methodName, object[] arguments);

        /// <summary>
        /// Convert the specified type into a string or a json string if
        /// it is something other than a string or number (int, decimal, long)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string EncodeValue(object value);

        string ArgumentsToQueryString(Dictionary<string, object> arguments);
    }
}
