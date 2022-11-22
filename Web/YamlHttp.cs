using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Web
{
    public class YamlHttp
    {
        public static Func<string, T> GetParser<T>()
        {
            return (s) => s.FromYaml<T>();
        }

        public static T Get<T>(string url, Dictionary<string, string> headers = null)
        {
            return Get<T>(url, null, headers);
        }

        /// <summary>
        /// Gets the specified url and parses the result as Json into an instance of the specified generic type T.
        /// </summary>
        /// <typeparam name="T">The type to parse the response as.</typeparam>
        /// <param name="url">The url.</param>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
        public static T Get<T>(string url, string userAgent, Dictionary<string, string> headers = null)
        {
            return Http.Get(url, GetParser<T>(), userAgent, headers);
        }
    }
}
