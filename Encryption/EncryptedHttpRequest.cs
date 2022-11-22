using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class EncryptedHttpRequest : HttpRequest, IEncryptedHttpRequest
    {        
        public Cipher ContentCipher { get; internal set; }
        public override string Content
        {
            get => this.ContentCipher;
            set => throw new InvalidOperationException("EncryptedHttpRequest.Content should not be set directly, use ContentCipher instead");
        }

        public override void Copy(IHttpRequest request)
        {
            this.Uri = request.Uri;
            this.Verb = request.Verb;
            foreach (string key in request.Headers.Keys)
            {
                this.Headers.Add(key, request.Headers[key]);
            }
        }
    }
}
