using Bam.Net.Encryption;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Web
{
    public class EncryptedServiceProxyResponseConverter : IResponseConverter
    {
        public ClientSessionInfo ClientSessionInfo { get; set; }

        public T ConvertResponse<T>(HttpClientResponse clientResponse)
        {
            AesKeyVectorPair aesKey = ClientSessionInfo.GetAesKey();
            string json = aesKey.Decrypt(clientResponse.Content);
            // execution request responses is always a SecureChannelResponseMessage where the data is the cipher of the execution result
            SecureChannelResponseMessage result = json.FromJson<SecureChannelResponseMessage>();
            if (result.Success)
            {
                EncryptedHttpClientResponse response = new EncryptedHttpClientResponse(ClientSessionInfo, (string)result.Data);
                return response.Content.FromJson<T>();
            }
            else
            {
                string properties = result.TryPropertiesToString();
                throw new ServiceProxyInvocationFailedException($"{result.Message}:\r\n\t{properties}");
            }
        }
    }
}
