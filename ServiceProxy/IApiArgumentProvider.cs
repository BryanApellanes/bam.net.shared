using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public interface IApiArgumentProvider<TService> : IApiArgumentProvider
    {
        HashSet<string> Methods { get; }
        Dictionary<string, object> GetNamedArguments(MethodInfo method, object[] arguments);
        Dictionary<string, object> GetNamedArguments(string methodName, object[] arguments);
    }

    public interface IApiArgumentProvider
    {
        string GetStringToHash(ExecutionRequest request);
        string GetStringToHash(string className, string methodName, string jsonArguments);

        string ArgumentsToJsonArgumentsObjectString(params object[] arguments);

        /// <summary>
        /// Convert the specified type into a string or a json string if
        /// it is something other than a string or number (int, decimal, long)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string GetArgumentFromValue(object value);

        string ArgumentsToNumberedQueryString(object[] arguments);
        string ArgumentsToQueryString(Dictionary<string, object> arguments);
    }
}
