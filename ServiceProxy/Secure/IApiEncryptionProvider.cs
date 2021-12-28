using Bam.Net.Server.ServiceProxy.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;

namespace Bam.Net.ServiceProxy.Secure
{
    public interface IApiEncryptionProvider
    {
        ISecureChannelSessionManager SecureChannelSessionManager { get; }
        void SetEncryptedValidationTokenHeaders(HttpRequestMessage request, string postString, string publicKey);

        EncryptedValidationToken ReadEncryptedValidationToken(NameValueCollection headers);

        EncryptedValidationToken CreateEncryptedValidationToken(SecureChannelSession session, string postString);

        EncryptedValidationToken CreateEncryptedValidationToken(string postString, string publicKeyPem);

        EncryptedValidationToken CreateEncryptedValidationToken(Instant instant, string postString, string publicKeyPem);

        EncryptedTokenValidationStatus ValidateEncryptedToken(IHttpContext context, string plainPost);

        EncryptedTokenValidationStatus ValidateEncryptedToken(NameValueCollection headers, string plainPost, bool usePkcsPadding = false);

        EncryptedTokenValidationStatus ValidateEncryptedToken(SecureChannelSession session, EncryptedValidationToken token, string plainPost, bool usePkcsPadding = false);

        EncryptedTokenValidationStatus ValidateHash(string nonce, string hash, string plainPost);

        EncryptedTokenValidationStatus ValidateNonce(string nonce, int offset);
    }
}
