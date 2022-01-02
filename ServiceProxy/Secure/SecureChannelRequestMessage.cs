﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;

namespace Bam.Net.ServiceProxy.Secure
{
    public class SecureChannelRequestMessage
    {
        public const string AsymetricCipherMediaType = "application/vnd+bam.cipher+asymetric";
        public const string SymetricCipherMediaType = "application/vnd+bam.cipher+symetric";

        public SecureChannelRequestMessage(ServiceProxyInvocationRequest serviceProxyInvokeRequest)
        {
            this.ClassName = serviceProxyInvokeRequest.ClassName;
            this.MethodName = serviceProxyInvokeRequest.MethodName;
            this.JsonArgs = serviceProxyInvokeRequest.ServiceProxyInvocationRequestArguments.GetJsonArgumentsArray().ToJson();
        }

        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string JsonArgs { get; set; }

        public StringContent GetSymetricCipherContent(ClientSession clientSessionInfo)
        {
            return new StringContent(GetSymetricCipher(clientSessionInfo), Encoding.UTF8, SymetricCipherMediaType);
        }

        public StringContent GetAsymetricCipherContent(ClientSession clientSessionInfo)
        {
            return new StringContent(GetAsymetricCipher(clientSessionInfo), Encoding.UTF8, AsymetricCipherMediaType);
        }

        /// <summary>
        /// Gets the symetric cipher of the current message.
        /// </summary>
        /// <param name="clientSessionInfo"></param>
        /// <returns></returns>
        public string GetSymetricCipher(ClientSession clientSessionInfo)
        {
            return clientSessionInfo.GetSymetricCipher(this.ToJson());
        }

        /// <summary>
        /// Gets the asymetric cipher of the current message.
        /// </summary>
        /// <param name="clientSessionInfo"></param>
        /// <returns></returns>
        public string GetAsymetricCipher(ClientSession clientSessionInfo)
        {
            return clientSessionInfo.GetAsymetricCipher(this.ToJson());
        }

        public void SetEncryptedValidationTokenHeaders(IApiEncryptionProvider encryptionProvider, ClientSession clientSessionInfo, HttpRequestMessage requestMessage)
        {
            encryptionProvider.SetEncryptedValidationTokenHeaders(requestMessage, this.ToJson(), clientSessionInfo.PublicKey);
        }
    }
}
