using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequestHeaderEncryptor : IHttpRequestHeaderEncryptor
    {
        public HttpRequestHeaderEncryptor(IEncryptor encryptor)
        {
            this.Encryptor = encryptor;
        }

        public IEncryptor Encryptor { get; private set; }

        public void EncryptHeaders(IHttpRequest request)
        {
            Args.ThrowIfNull(request, nameof(request));
            if(request.Headers == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(request.ContentType))
            {
                request.Headers.Add("Content-Type-Cipher", Encryptor.EncryptString(request.ContentType));
            }
            foreach(string header in HttpHeaders.PlainHeaders)
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
