using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequestHeaderEncryptor : IHttpRequestHeaderEncryptor
    {
        public static List<string> PlainHeders = new List<string>()
        {
            Headers.ProcessLocalIdentifier,
            Headers.ProcessDescriptor,
            Headers.ProcessMode,
            Headers.ApplicationName,
            Headers.Hash
        };

        public HttpRequestHeaderEncryptor(IEncryptor encryptor)
        {
            this.Encryptor = encryptor;
        }

        public IEncryptor Encryptor { get; private set; }

        public void EncryptHeaders(IHttpRequest request)
        {
            Args.ThrowIfNull(request, nameof(request));
            foreach(string header in PlainHeders)
            {
                if (request.Headers.ContainsKey(header))
                {
                    string plainHeaderValue = request.Headers[header];
                    request.Headers.Remove(header);
                    request.Headers.Add($"{header}-Cipher", Encryptor.EncryptString(plainHeaderValue));
                }
            }
        }
    }
}
