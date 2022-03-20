using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequestHeaderDecryptor : IHttpRequestHeaderDecryptor
    {
        public HttpRequestHeaderDecryptor(IDecryptor decryptor)
        {
            this.Decryptor = decryptor;
        }

        public IDecryptor Decryptor { get; private set; }

        public void DecryptHeaders(IHttpRequest request)
        {
            Args.ThrowIfNull(request, nameof(request));
            foreach(string header in HttpHeaders.CipherHeaders)
            {
                if(request.Headers.ContainsKey(header))
                {
                    string cipherHeaderValue = request.Headers[header];
                    request.Headers.Remove(header);
                    request.Headers.Add(header.Truncate("-Cipher".Length), Decryptor.DecryptString(cipherHeaderValue));
                }
            }
        }
    }
}
