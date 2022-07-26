using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Data;
using Bam.Net.ServiceProxy.Encryption;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server.ServiceProxy
{
    public class EncryptedServiceProxyInvocationArgumentReader : ServiceProxyInvocationArgumentReader
    {
        public EncryptedServiceProxyInvocationArgumentReader(ISecureChannelSessionDataManager secureChannelSessionDataManager)
        {
            this.SecureChannelSessionDataManager = secureChannelSessionDataManager;
        }

        [Inject]
        public ISecureChannelSessionDataManager SecureChannelSessionDataManager { get; set; }

        public override async Task<ServiceProxyInvocationArgument[]> ReadArgumentsAsync(MethodInfo methodInfo, IHttpContext httpContext)
        {
            SecureChannelSession session = await SecureChannelSessionDataManager.GetSecureChannelSessionForContextAsync(httpContext);
            string cipher = httpContext.Request.Content;

            // read ContentType to determine appropriate Encryptor/Decryptor
            // Use SymetricDataEncryptor.GetDecryptor
            // implement IHttpRequestDecryptor
            string body = session.GetClientSession().GetAesKey().Decrypt(cipher);
            return await Task.FromResult(ReadJsonArgumentsMember(methodInfo, body));
        }
    }
}
