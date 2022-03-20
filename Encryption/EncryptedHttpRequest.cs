using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class EncryptedHttpRequest : HttpRequest, IEncryptedHttpRequest
    {
        public override string Content 
        {
            get => ContentCipher;
            set => ContentCipher = value;
        }

        public Cipher ContentCipher { get; internal set; }

        public override void Copy(IHttpRequest request)
        {
            this.ContentType = request.ContentType;
            this.Verb = request.Verb;
            foreach (string key in request.Headers.Keys)
            {
                this.Headers.Add(key, request.Headers[key]);
            }
        }
    }
}
