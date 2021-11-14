/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Bam.Net.Encryption;
using Bam.Net.Logging;
using Bam.Net.Web;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Bam.Net.ServiceProxy.Secure
{
    /// <summary>
    /// Class used to set and validate encryption validation
    /// tokens.
    /// </summary>
    internal class ApiEncryptionValidation
    {
        public static void SetEncryptedValidationTokenHeaders(HttpRequestMessage request, string plainPostString, string publicKeyPem)
        {
            SetEncryptedValidationTokenHeaders(request.Headers, plainPostString, publicKeyPem);
        }

        [Obsolete("Use SetEncryptedValidationTokenHeaders instead.")]
        public static void SetEncryptedValidationToken(NameValueCollection headers, string plainPostString, string publicKeyPem)
        {
            EncryptedValidationToken token = CreateEncryptedValidationToken(plainPostString, publicKeyPem);
            headers[Headers.Nonce] = token.NonceCipher;
            headers[Headers.ValidationToken] = token.HashCipher;
        }
        
        public static void SetEncryptedValidationTokenHeaders(HttpRequestHeaders headers, string plainPostString, string publicKeyPem)
        {
            EncryptedValidationToken token = CreateEncryptedValidationToken(plainPostString, publicKeyPem);
            headers.Add(Headers.Nonce, token.NonceCipher);
            headers.Add(Headers.ValidationToken, token.HashCipher);
        }

        public static EncryptedValidationToken ReadEncryptedValidationToken(NameValueCollection headers)
        {
            EncryptedValidationToken result = new EncryptedValidationToken
            {
                NonceCipher = headers[Headers.Nonce],
                HashCipher = headers[Headers.ValidationToken]
            };
            Args.ThrowIfNull(result.NonceCipher, Headers.Nonce);
            Args.ThrowIf<EncryptionValidationTokenNotFoundException>(
                string.IsNullOrEmpty(result.HashCipher),  
                "Header was not found: {0}",
                Headers.ValidationToken);
            return result;
        }

        public static EncryptedValidationToken CreateEncryptedValidationToken(string postString, SecureSession session)
        {
            return CreateEncryptedValidationToken(postString, session.PublicKey);
        }

        public static EncryptedValidationToken CreateEncryptedValidationToken(string postString, string publicKeyPem)
        {            
            Instant instant = new Instant();
            return CreateEncryptedValidationToken(instant, postString, publicKeyPem);
        }

        public static EncryptedValidationToken CreateEncryptedValidationToken(Instant instant, string validatedString, string publicKeyPem, HashAlgorithms algorithm = HashAlgorithms.SHA256)
        {
            //{Month}/{Day}/{Year};{Hour}.{Minute}.{Second}.{Millisecond}:{PostString}
            string nonce = instant.ToString();
            string hash = $"{nonce}:{validatedString}".HashHexString(algorithm);
            string hashCipher = hash.EncryptWithPublicKey(publicKeyPem);
            string nonceCipher = nonce.EncryptWithPublicKey(publicKeyPem);

            return new EncryptedValidationToken { HashCipher = hashCipher, NonceCipher = nonceCipher };
        }

        public static EncryptedTokenValidationStatus ValidateEncryptedToken(IHttpContext context, string post)
        {
            NameValueCollection headers = context.Request.Headers;
            
            string paddingValue = headers[Headers.Padding] ?? string.Empty;
            bool usePadding = paddingValue.ToLowerInvariant().Equals("true");
            
            return ValidateEncryptedToken(headers, post, usePadding);
        }

        public static EncryptedTokenValidationStatus ValidateEncryptedToken(NameValueCollection headers, string plainPost, bool usePkcsPadding = false)
        {
            SecureSession session = SecureSession.Get(headers);
            EncryptedValidationToken token = ReadEncryptedValidationToken(headers);

            return ValidateEncryptedToken(session, token, plainPost, usePkcsPadding);
        }

        public static EncryptedTokenValidationStatus ValidateEncryptedToken(SecureSession session, EncryptedValidationToken token, string plainPost, bool usePkcsPadding = false)
        {
            Args.ThrowIfNull(session, "session");
            Args.ThrowIfNull(token, "token");

            return ValidateEncryptedToken(session, token.HashCipher, token.NonceCipher, plainPost, usePkcsPadding);
        }

        public static EncryptedTokenValidationStatus ValidateEncryptedToken(SecureSession session, string hashCipher, string nonceCipher, string plainPost, bool usePkcsPadding = false)
        {
            string hash = session.DecryptWithPrivateKey(hashCipher, usePkcsPadding);
            string nonce = session.DecryptWithPrivateKey(nonceCipher, usePkcsPadding);

            int offset = session.TimeOffset.Value;

            EncryptedTokenValidationStatus result = ValidateNonce(nonce, offset);
            if (result == EncryptedTokenValidationStatus.Success)
            {
                result = ValidateHash(nonce, hash, plainPost);
            }

            return result;
        }

        public static EncryptedTokenValidationStatus ValidateHash(string nonce, string hash, string plainPost)
        {
            string kvpFormat = "{0}:{1}";
            string checkHash = kvpFormat._Format(nonce, plainPost).Sha256();
            EncryptedTokenValidationStatus result = EncryptedTokenValidationStatus.HashFailed;
            if (checkHash.Equals(hash))
            {
                result = EncryptedTokenValidationStatus.Success;
            }

            return result;
        }

        /// <summary>
        /// Checks that the specified nonce is no more than
        /// 3 minutes in the past or future
        /// </summary>
        /// <param name="nonce"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static EncryptedTokenValidationStatus ValidateNonce(string nonce, int offset)
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
