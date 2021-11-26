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

namespace Bam.Net.ServiceProxy
{
    /// <summary>
    /// A class used to properly format parameters for service proxy calls.
    /// </summary>
    public class ApiArgumentEncoder
    {
        public static string GetStringToHash(ServiceProxyInvocation request)
        {
            return GetStringToHash(request.ClassName, request.MethodName, "");// request.ArgumentsAsJsonArrayOfJsonStrings);
        }

        public static string GetStringToHash(string className, string methodName, string jsonArgsArray)
        {
            return string.Format("{0}.{1}.{2}", className, methodName, jsonArgsArray);
        }

        /// <summary>
        /// Convert the specified arguments to an array of json strings set as the value of a
        /// property named `jsonArgs` of a json serialized anonymous object.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
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
        /// Returns an array of json strings that represent each parameter.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static string[] ArgumentsToJsonArgumentsArray(params object[] arguments)
        {
            // create a string array
            string[] jsonArgs = new string[arguments.Length];

            JsonSerializerSettings settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            // for each parameter stringify it and put it into the array
            arguments.Each((o, i) => jsonArgs[i] = o.ToJson(settings));
            return jsonArgs;
        }
    }
}
