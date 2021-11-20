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
using Bam.Net.ServiceProxy;
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

namespace Bam.Net.ServiceProxy
{
    /// <summary>
    /// A class used to properly format parameters for service proxy calls.
    /// </summary>
    public class ApiArguments
    {
        public static string GetStringToHash(ServiceProxyInvocation request)
        {
            return GetStringToHash(request.ClassName, request.MethodName, request.ArgumentsAsJsonArrayOfJsonStrings);
        }

        public static string GetStringToHash(string className, string methodName, string jsonArgs)
        {
            return string.Format("{0}.{1}.{2}", className, methodName, jsonArgs);
        }

        public static string ArgumentsToJsonArgsMember(params object[] arguments)
        {
            string[] jsonArguments = ArgumentsToJsonArgumentsArray(arguments);
            string jsonArgumentsString = (new
            {
                jsonArgs = jsonArguments.ToJson()
            }).ToJson();

            return jsonArgumentsString;
        }

        /// <summary>
        /// Turn the specified parameter array into a JSON object in the form {jsonParams: &lt;json string array&gt;}.
        /// Where &lt;json string array&gt; can be obtained by callig ParametersToJsonParamsArray(parameters)
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("Use ArgumentsToJsonArgumentsObjectString instead.")]
        public static string ParametersToJsonParamsObjectString(params object[] parameters)
        {
            string[] jsonArguments = ArgumentsToJsonArgumentsArray(parameters);

            // stringify the array
            string jsonParamsString = (new { jsonParams = jsonArguments.ToJson() }).ToJson();
            return jsonParamsString;
        }

        /// <summary>
        /// Returns an array of json strings that represent each parameter.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static string[] ArgumentsToJsonArgumentsArray(params object[] arguments)
        {
            // create a string array
            string[] jsonArgs = new string[arguments.Length];

            JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            // for each parameter stringify it and shove it into the array
            arguments.Each((o, i) => jsonArgs[i] = o.ToJson(settings));
            return jsonArgs;
        }
    }
}
