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
using Bam.Net.ServiceProxy.Data;

namespace Bam.Net.ServiceProxy.Encryption
{
    /// <summary>
    /// Class used to set and validate encryption validation
    /// tokens.
    /// </summary>
    internal class ApiValidation // TODO: move this implementation to ApiEncryptionProvider 
    {

        public static EncryptedValidationToken ReadEncryptedValidationToken(NameValueCollection headers)
        {
            EncryptedValidationToken result = new EncryptedValidationToken
            {
                TimestampCipher = headers[CipherHeaders.TimestampCipher],
                HashCipher = headers[CipherHeaders.HashCipher]
            };
            Args.ThrowIfNull(result.TimestampCipher, CipherHeaders.TimestampCipher);
            Args.ThrowIf<EncryptedValidationTokenNotFoundException>(
                string.IsNullOrEmpty(result.HashCipher),  
                "Header was not found: {0}",
                CipherHeaders.HashCipher);
            return result;
        }

        public static EncryptedValidationToken CreateEncryptedValidationToken(string postString, SecureSession session)
        {
            return CreateEncryptedValidationToken(postString, session.PublicKey);
        }

        public static EncryptedValidationToken CreateEncryptedValidationToken(string postString, SecureChannelSession session)
        {
            return CreateEncryptedValidationToken(postString, session.GetPublicKey());
        }

        public static EncryptedValidationToken CreateEncryptedValidationToken(string postString, string publicKeyPem)
        {            
            Instant instant = new Instant();
            return CreateEncryptedValidationToken(instant, postString, publicKeyPem);
        }

        public static EncryptedValidationToken CreateEncryptedValidationToken(Instant instant, string validatedString, string publicKeyPem, HashAlgorithms algorithm = HashAlgorithms.SHA256)
        {
            //{Month}/{Day}/{Year};{Hour}.{Minute}.{Second}.{Millisecond}
            string timestamp = instant.ToString();
            string hash = $"{timestamp}:{validatedString}".HashHexString(algorithm);            
            string hashCipher = hash.EncryptWithPublicKey(publicKeyPem);
            string timestampCipher = timestamp.EncryptWithPublicKey(publicKeyPem);

            return new EncryptedValidationToken { HashCipher = hashCipher, TimestampCipher = timestampCipher };
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

            return ValidateEncryptedToken(session, token.HashCipher, token.TimestampCipher, plainPost, usePkcsPadding);
        }

        public static EncryptedTokenValidationStatus ValidateEncryptedToken(SecureChannelSession session, EncryptedValidationToken token, string plainPost, bool usePkcsPadding = false)
        {
            Args.ThrowIfNull(session, "session");
            Args.ThrowIfNull(token, "token");

            //return new ValidationToken(plainPost)
            return ValidateEncryptedToken(session, token.HashCipher, token.TimestampCipher, plainPost, usePkcsPadding);
        }

        public static EncryptedTokenValidationStatus ValidateEncryptedToken(SecureSession session, string hashCipher, string nonceCipher, string plainPost, bool usePkcsPadding = false)
        {
            string hash = session.DecryptWithPrivateKey(hashCipher, usePkcsPadding);
            string nonce = session.DecryptWithPrivateKey(nonceCipher, usePkcsPadding);

            int offset = session.TimeOffset.Value;

            EncryptedTokenValidationStatus result = ValidateNonce(nonce, offset);
            if (result == EncryptedTokenValidationStatus.Success)
            {
                result = ValidateHash(plainPost, nonce, hash);
            }

            return result;
        }

        public static EncryptedTokenValidationStatus ValidateEncryptedToken(SecureChannelSession session, string hashCipher, string nonceCipher, string plainPost, bool usePkcsPadding = false)
        {
            string hash = session.DecryptWithPrivateKey(hashCipher, usePkcsPadding);
            string nonce = session.DecryptWithPrivateKey(nonceCipher, usePkcsPadding);

            int offset = session.TimeOffset.Value;
            EncryptedTokenValidationStatus encryptedTokenValidationStatus = ValidateNonce(nonce, offset);
            if(encryptedTokenValidationStatus == EncryptedTokenValidationStatus.Success)
            {
                encryptedTokenValidationStatus = ValidateHash(plainPost, nonce, hash);
            }

            return encryptedTokenValidationStatus;
        }

        public static EncryptedTokenValidationStatus ValidateHash(string plainPost, string nonce, string hash)
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
        /// 3 minutes in the past or future.
        /// </summary>
        /// <param name="nonce"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [Obsolete("Use ApiValidationProvider.ValidateNonce instead")]
        public static EncryptedTokenValidationStatus ValidateNonce(string nonce, int offset)
        {
            return new ApiValidationProvider(null).ValidateNonce(nonce, offset);
        }
    }
}
