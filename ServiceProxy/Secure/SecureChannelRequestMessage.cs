using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;

namespace Bam.Net.ServiceProxy.Secure
{
    public class SecureChannelRequestMessage<TService>
    {
        public const string AsymetricCipherMediaType = "application/vnd.bam+cipher;algorithm=asymetric";

        public SecureChannelRequestMessage(ServiceProxyInvokeRequest serviceProxyInvokeRequest)
        {
            this.ClassName = serviceProxyInvokeRequest.ClassName;
            this.MethodName = serviceProxyInvokeRequest.MethodName;
            this.JsonArgs = serviceProxyInvokeRequest.ServiceProxyArguments.GetJsonArgumentsArray().ToJson();
        }

        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string JsonArgs { get; set; }

        public StringContent GetContent(ClientSessionInfo clientSessionInfo)
        {
            return new StringContent(GetCipher(clientSessionInfo), Encoding.UTF8, AsymetricCipherMediaType);
        }

        /// <summary>
        /// Gets the cipher of the current message.
        /// </summary>
        /// <param name="clientSessionInfo"></param>
        /// <returns></returns>
        public string GetCipher(ClientSessionInfo clientSessionInfo)
        {
            return clientSessionInfo.Encrypt(this.ToJson());
        }

        public void SetEncryptedValidationTokenHeaders(IApiEncryptionProvider encryptionProvider, ClientSessionInfo clientSessionInfo, HttpRequestMessage requestMessage)
        {
            encryptionProvider.SetEncryptedValidationTokenHeaders(requestMessage, this.ToJson(), clientSessionInfo.PublicKey);
        }
    }
}
