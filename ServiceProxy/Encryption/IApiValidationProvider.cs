using Bam.Net.ServiceProxy.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;

namespace Bam.Net.ServiceProxy.Encryption
{
    [Obsolete("Use Encryption.HttpRequestHeaderWriter")]
    public interface IApiValidationProvider
    {
        ISecureChannelSessionDataManager SecureChannelSessionDataManager { get; }
        void SetEncryptedValidationTokenHeaders(HttpRequestMessage request, string postString, string publicKey);

        EncryptedValidationToken ReadEncryptedValidationToken(NameValueCollection headers);

        EncryptedValidationToken CreateEncryptedValidationToken(SecureChannelSession session, string postString);

        EncryptedValidationToken CreateEncryptedValidationToken(string postString, string publicKeyPem);

        EncryptedValidationToken CreateEncryptedValidationToken(Instant instant, string postString, string publicKeyPem, HashAlgorithms hashAlgorithm = HashAlgorithms.SHA256);

        EncryptedTokenValidationStatus ValidateEncryptedToken(IHttpContext context, string plainPost);

        
        EncryptedTokenValidationStatus ValidateEncryptedToken(SecureChannelSession session, EncryptedValidationToken token, string plainPost, bool usePkcsPadding = false);

        EncryptedTokenValidationStatus ValidateHash(string plainPost, string nonce, string hash);

        EncryptedTokenValidationStatus ValidateNonce(string nonce, int offset);
    }
}
