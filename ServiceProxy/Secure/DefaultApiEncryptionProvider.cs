﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;

namespace Bam.Net.ServiceProxy.Secure
{
    public class DefaultApiEncryptionProvider : IApiEncryptionProvider
    {
        public EncryptedValidationToken CreateEncryptedValidationToken(string postString, SecureSession session)
        {
            return ApiEncryptionValidation.CreateEncryptedValidationToken(postString, session);
        }

        public EncryptedValidationToken CreateEncryptedValidationToken(string postString, string publicKeyPem)
        {
            return ApiEncryptionValidation.CreateEncryptedValidationToken(postString, publicKeyPem);
        }

        public EncryptedValidationToken CreateEncryptedValidationToken(Instant instant, string postString, string publicKeyPem)
        {
            return ApiEncryptionValidation.CreateEncryptedValidationToken(instant, postString, publicKeyPem);
        }

        public EncryptedValidationToken ReadEncryptedValidationToken(NameValueCollection headers)
        {
            return ApiEncryptionValidation.ReadEncryptedValidationToken(headers);
        }

        /// <summary>
        /// Sets the Nonce (X-Bam-Timestamp) and ValidationToken (X-Bam-Validation-Token) headers on the specified request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validatedString"></param>
        /// <param name="publicKey"></param>
        public void SetEncryptedValidationTokenHeaders(HttpRequestMessage request, string validatedString, string publicKey)
        {
            ApiEncryptionValidation.SetEncryptedValidationTokenHeaders(request, validatedString, publicKey);
        }

        public EncryptedTokenValidationStatus ValidateEncrtypedToken(SecureSession session, string hashCipher, string nonceCipher, string plainPost, bool usePkcsPadding = false)
        {
            return ApiEncryptionValidation.ValidateEncryptedToken(session, hashCipher, nonceCipher, plainPost, usePkcsPadding);
        }

        public EncryptedTokenValidationStatus ValidateEncryptedToken(IHttpContext context, string plainPost)
        {
            return ApiEncryptionValidation.ValidateEncryptedToken(context, plainPost);
        }

        public EncryptedTokenValidationStatus ValidateEncryptedToken(NameValueCollection headers, string plainPost, bool usePkcsPadding = false)
        {
            return ApiEncryptionValidation.ValidateEncryptedToken(headers, plainPost, usePkcsPadding);
        }

        public EncryptedTokenValidationStatus ValidateEncryptedToken(SecureSession session, EncryptedValidationToken token, string plainPost, bool usePkcsPadding = false)
        {
            return ApiEncryptionValidation.ValidateEncryptedToken(session, token, plainPost, usePkcsPadding);
        }

        public EncryptedTokenValidationStatus ValidateHash(string nonce, string hash, string plainPost)
        {
            return ApiEncryptionValidation.ValidateHash(nonce, hash, plainPost);
        }

        public EncryptedTokenValidationStatus ValidateNonce(string nonce, int offset)
        {
            return ApiEncryptionValidation.ValidateNonce(nonce, offset);
        }
    }
}
