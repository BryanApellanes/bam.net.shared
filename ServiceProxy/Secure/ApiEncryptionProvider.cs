using Bam.Net.ServiceProxy.Data;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Bam.Net.ServiceProxy.Secure
{
    public class ApiEncryptionProvider : IApiEncryptionProvider
    {
        public ApiEncryptionProvider(ISecureChannelSessionDataManager secureChannelSessionManager)
        {
            this.SecureChannelSessionDataManager = secureChannelSessionManager;
        }

        public ISecureChannelSessionDataManager SecureChannelSessionDataManager { get; private set; }

        public EncryptedValidationToken CreateEncryptedValidationToken(SecureChannelSession session, string postString)
        {
            return ApiEncryptionValidation.CreateEncryptedValidationToken(postString, session);
        }

        public EncryptedValidationToken CreateEncryptedValidationToken(string postString, string publicKeyPem)
        {
            return ApiEncryptionValidation.CreateEncryptedValidationToken(postString, publicKeyPem);
        }

        public EncryptedValidationToken CreateEncryptedValidationToken(Instant instant, string postString, string publicKeyPem, HashAlgorithms hashAlgorithm = HashAlgorithms.SHA256)
        {
            return new ValidationToken(postString, instant.ToString(), hashAlgorithm).Encrypt(publicKeyPem);
        }

        /// <summary>
        /// Returns a value indicating if the specified value and nonce hash to the same hash value specified.
        /// </summary>
        /// <param name="plainPost"></param>
        /// <param name="nonce"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public EncryptedTokenValidationStatus ValidateHash(string plainPost, string nonce, string hash)
        {
            ValidationToken validationToken = new ValidationToken(plainPost, nonce);
            if(validationToken.Hash.Equals(hash))
            {
                return EncryptedTokenValidationStatus.Success;
            }
            return EncryptedTokenValidationStatus.HashFailed;
        }

        public EncryptedValidationToken ReadEncryptedValidationToken(NameValueCollection headers)
        {
            EncryptedValidationToken encryptedValidationToken = new EncryptedValidationToken
            {
                NonceCipher = headers[Headers.Nonce],
                HashCipher = headers[Headers.ValidationToken]
            };
            Args.ThrowIfNull(encryptedValidationToken.NonceCipher, Headers.Nonce);
            Args.ThrowIf<EncryptionValidationTokenNotFoundException>
                (
                    string.IsNullOrEmpty(encryptedValidationToken.HashCipher),
                    "Header was not found: {0}",
                    Headers.ValidationToken
                );

            return encryptedValidationToken;
        }

        /// <summary>
        /// Sets the Nonce (X-Bam-Timestamp) and ValidationToken (X-Bam-Validation-Token) headers on the specified request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="plainPostString"></param>
        /// <param name="publicKeyPem"></param>
        public void SetEncryptedValidationTokenHeaders(HttpRequestMessage request, string plainPostString, string publicKeyPem)
        {
            HttpRequestHeaders headers = request.Headers;
            EncryptedValidationToken token = CreateEncryptedValidationToken(plainPostString, publicKeyPem);
            headers.Add(Headers.Nonce, token.NonceCipher);
            headers.Add(Headers.ValidationToken, token.HashCipher);
        }

        public EncryptedTokenValidationStatus ValidateEncryptedToken(IHttpContext context, string plainPost)
        {
            NameValueCollection headers = context.Request?.Headers;
            Args.ThrowIfNull(headers, "context.Request.Headers");

            bool usePadding = (bool)headers[Headers.Padding]?.IsAffirmative();

            SecureChannelSession secureChannelSession = SecureChannelSessionDataManager.GetSecureChannelSessionForContextAsync(context).Result;
            EncryptedValidationToken encryptedValidationToken = ReadEncryptedValidationToken(context.Request.Headers);

            return ValidateEncryptedToken(secureChannelSession, encryptedValidationToken, plainPost, usePadding);
        }

        public EncryptedTokenValidationStatus ValidateEncryptedToken(SecureChannelSession session, EncryptedValidationToken encryptedToken, string plainPost, bool usePkcsPadding = false)
        {
            string hash = session.DecryptWithPrivateKey(encryptedToken.HashCipher, usePkcsPadding);
            string nonce = session.DecryptWithPrivateKey(encryptedToken.NonceCipher, usePkcsPadding);
            int offset = session.TimeOffset.Value;
            EncryptedTokenValidationStatus encryptedTokenValidationStatus = ValidateNonce(nonce, offset);
            if(encryptedTokenValidationStatus == EncryptedTokenValidationStatus.Success)
            {
                encryptedTokenValidationStatus = ValidateHash(plainPost, nonce, hash);
            }

            return encryptedTokenValidationStatus;
        }

        /// <summary>
        /// Checks that the specified nonce is no more than
        /// 3 minutes in the past or future.
        /// </summary>
        /// <param name="nonce"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public EncryptedTokenValidationStatus ValidateNonce(string nonce, int offset)
        {
            EncryptedTokenValidationStatus result = EncryptedTokenValidationStatus.Success;
            Instant requestInstant = Instant.FromString(nonce);
            Instant currentInstant = new Instant();

            int difference = currentInstant.DiffInMilliseconds(requestInstant);
            difference = difference - offset;
            if (TimeSpan.FromMilliseconds(difference).TotalMinutes > 3)
            {
                result = EncryptedTokenValidationStatus.NonceFailed;
            }
            return result;
        }
    }
}
