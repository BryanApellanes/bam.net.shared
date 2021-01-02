using System.Collections.Generic;

namespace Bam.Net.CoreServices.AccessControl
{
    public class AuthorizationHeader
    {
        public const string DefaultKey = "Authorization";
        public AuthorizationHeader()
        {
            Key = DefaultKey;
        }
        
        public virtual string Key { get; set; }
        
        public virtual string Value { get; set; }
        
        /// <summary>
        /// Add this header to the specified dictionary.
        /// </summary>
        /// <param name="headers"></param>
        public void Add(Dictionary<string, string> headers)
        {
            headers.Add(Key, Value);
        }

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>()
            {
                {Key, Value}
            };
        }
    }
}