using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class SecureChannelRequestMessage
    {
        public SecureChannelRequestMessage() { }

        public SecureChannelRequestMessage(ServiceProxyInvocationRequest serviceProxyInvokeRequest)
        {
            this.ClassName = serviceProxyInvokeRequest.ClassName;
            this.MethodName = serviceProxyInvokeRequest.MethodName;
            this.JsonArgs = serviceProxyInvokeRequest.ServiceProxyInvocationRequestArgumentWriter.GetJsonArgumentsArray().ToJson();
        }

        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string JsonArgs { get; set; }

        public StringContent GetSymetricCipherContent(ClientSessionInfo clientSessionInfo)
        {
            return new StringContent(GetSymetricCipher(clientSessionInfo), Encoding.UTF8, ContentTypes.SymmetricCipher);
        }

        public StringContent GetAsymetricCipherContent(ClientSessionInfo clientSessionInfo)
        {
            return new StringContent(GetAsymetricCipher(clientSessionInfo), Encoding.UTF8, ContentTypes.AsymmetricCipher);
        }

        /// <summary>
        /// Gets the symetric cipher of the current message.
        /// </summary>
        /// <param name="clientSessionInfo"></param>
        /// <returns></returns>
        public string GetSymetricCipher(ClientSessionInfo clientSessionInfo)
        {
            return clientSessionInfo.GetSymetricCipher(this.ToJson());
        }

        /// <summary>
        /// Gets the asymetric cipher of the current message.
        /// </summary>
        /// <param name="clientSessionInfo"></param>
        /// <returns></returns>
        public string GetAsymetricCipher(ClientSessionInfo clientSessionInfo)
        {
            return clientSessionInfo.GetAsymetricCipher(this.ToJson());
        }

        public void SetEncryptedValidationTokenHeaders(IApiEncryptionProvider encryptionProvider, ClientSessionInfo clientSessionInfo, HttpRequestMessage requestMessage)
        {
            encryptionProvider.SetEncryptedValidationTokenHeaders(requestMessage, this.ToJson(), clientSessionInfo.PublicKey);
        }
    }
}
