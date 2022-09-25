using Bam.Net.Encryption;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Data;
using Bam.Net.ServiceProxy.Encryption;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server.ServiceProxy
{
    public class EncryptedServiceProxyInvocationArgumentReader : ServiceProxyInvocationArgumentReader
    {
        MethodInfo _secureChannelInvoke;
        ParameterInfo _secureChannelParameter;

        public EncryptedServiceProxyInvocationArgumentReader(ISecureChannelSessionDataManager secureChannelSessionDataManager)
        {
            this._secureChannelInvoke = typeof(SecureChannel).GetMethod(nameof(SecureChannel.Execute));
            this._secureChannelParameter = _secureChannelInvoke.GetParameters().First();
            this.SecureChannelSessionDataManager = secureChannelSessionDataManager;
        }

        [Inject]
        public ISecureChannelSessionDataManager SecureChannelSessionDataManager { get; set; }

        public override ServiceProxyInvocation CreateServiceProxyInvocation(WebServiceProxyDescriptors webServiceProxyDescriptors, string className, string methodName, IHttpContext context)
        {
            return base.CreateServiceProxyInvocation(webServiceProxyDescriptors, className, methodName, context);
        }

        public override async Task<ServiceProxyInvocationArgument[]> ReadArgumentsAsync(MethodInfo methodInfo, IHttpContext httpContext)
        {
            Args.ThrowIf(methodInfo != _secureChannelInvoke, "Invalid method specified, must be SecureChannel.Invoke");

            SecureChannelSession session = await SecureChannelSessionDataManager.GetSecureChannelSessionForContextAsync(httpContext);
            ClientSessionInfo clientSessionInfo = session.GetClientSession();
            SymmetricContentDecryptor<SecureChannelRequestMessage> contentDecryptor = new SymmetricContentDecryptor<SecureChannelRequestMessage>(clientSessionInfo);

            string cipher = httpContext.Request.Content;
            SecureChannelRequestMessage secureChannelRequestMessage = contentDecryptor.Decrypt(new Cipher<SecureChannelRequestMessage>(cipher));

            return new ServiceProxyInvocationArgument[] { new ServiceProxyInvocationArgument(this, _secureChannelParameter, secureChannelRequestMessage) };
        }

        internal IEnumerable<ServiceProxyInvocationArgument> GetServiceProxyInvocationArguments(SecureChannelRequestMessage secureChannelRequestMessage, MethodInfo methodInfo)
        {
            List<ParameterInfo> parameters = methodInfo.GetParameters().ToList();
            if (parameters.Count == 0)
            {
                yield break;
            }

            parameters.Sort((x, y) => x.Position.CompareTo(y.Position));

            string[] arrayOfJsonArguments = secureChannelRequestMessage.JsonArgs.FromJson<string[]>();

            for (int i = 0; i < parameters.Count; i++)
            {
                ParameterInfo parameter = parameters[i];
                string jsonArgument = arrayOfJsonArguments[i];
                yield return new ServiceProxyInvocationArgument(this, parameter, jsonArgument);
            }
        }
    }
}
