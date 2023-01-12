using bam.net.shared.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class HttpRequestDecryptor<TContent> : HttpRequestDecryptor, IHttpRequestDecryptor<TContent>
    {
        public HttpRequestDecryptor(IContentDecryptor<TContent> decryptor) : base(decryptor)
        {
        }

        public HttpRequestDecryptor(IContentDecryptor<TContent> contentDecrpytor, IDecryptor headerDecryptor) : base(contentDecrpytor, headerDecryptor)
        {
            this.ContentDecryptor = contentDecrpytor;
        }

        public new IContentDecryptor<TContent> ContentDecryptor
        {
            get;
            private set;
        }

        public IHttpRequest<TContent> DecryptRequest(IEncryptedHttpRequest<TContent> request)
        {
            HttpRequest<TContent> copy = new HttpRequest<TContent>();
            copy.Verb = request.Verb;
            foreach(string key in request.Headers.Keys)
            {
                copy.Headers.Add(key, request.Headers[key]);
            }
            copy.Headers.Add("Content-Type", MediaTypes.Json);
            copy.TypedContent = ContentDecryptor.DecryptContentCipher(request.ContentCipher);
            return copy;
        }
    }
}
