/**
This file was generated from http://core.bamapps.net/serviceproxy/csharpproxies.  This file should not be modified directly
**/


namespace Bam.Net.CoreServices
{
	using System;
	using Bam.Net.Configuration;
	using Bam.Net.ServiceProxy;
	using Bam.Net.ServiceProxy.Encryption;
	using Bam.Net.CoreServices.Contracts;
	using Bam.Net.CoreServices;
	using Bam.Net.CoreServices.ApplicationRegistration.Data;
	using System.Collections.Generic;
	using Bam.Net.UserAccounts;

    
    public class ApplicationRegistryServiceClient: EncryptedServiceProxyClient<Bam.Net.CoreServices.Contracts.IApplicationRegistryService>, Bam.Net.CoreServices.Contracts.IApplicationRegistryService
    {
        public ApplicationRegistryServiceClient(): base(DefaultConfiguration.GetAppSetting("ApplicationRegistrationServiceUrl", "http://core.bamapps.net/"))
        {
        }

        public ApplicationRegistryServiceClient(string baseAddress): base(baseAddress)
        {
        }
        
        
		[ApiSigningKeyRequired]
        public ApiSigningKeyInfo[] ListApiKeys()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<ApiSigningKeyInfo[]>("ListApiKeys", parameters);
        }
		[ApiSigningKeyRequired]
        public ApiSigningKeyInfo AddApiKey()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<ApiSigningKeyInfo>("AddApiKey", parameters);
        }
		[ApiSigningKeyRequired]
        public ApiSigningKeyInfo SetActiveApiKeyIndex(System.Int32 index)
        {
            object[] parameters = new object[] { index };
            return InvokeServiceMethod<ApiSigningKeyInfo>("SetActiveApiKeyIndex", parameters);
        }
        public String GetApplicationName()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<String>("GetApplicationName", parameters);
        }
        public ApiSigningKeyInfo GetClientApiKeyInfo()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<ApiSigningKeyInfo>("GetClientApiKeyInfo", parameters);
        }
        public CoreServiceResponse RegisterApplication(System.String applicationName)
        {
            object[] parameters = new object[] { applicationName };
            return InvokeServiceMethod<CoreServiceResponse>("RegisterApplication", parameters);
        }
        public CoreServiceResponse RegisterApplicationProcess(Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor descriptor)
        {
            object[] parameters = new object[] { descriptor };
            return InvokeServiceMethod<CoreServiceResponse>("RegisterApplicationProcess", parameters);
        }
        public CoreServiceResponse RegisterClient(Bam.Net.CoreServices.ApplicationRegistration.Data.Client client)
        {
            object[] parameters = new object[] { client };
            return InvokeServiceMethod<CoreServiceResponse>("RegisterClient", parameters);
        }
        public Dictionary<System.String, System.String> GetSettings()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<Dictionary<System.String, System.String>>("GetSettings", parameters);
        }
        public LoginResponse ConnectClient(Bam.Net.CoreServices.ApplicationRegistration.Data.Client client)
        {
            object[] parameters = new object[] { client };
            return InvokeServiceMethod<LoginResponse>("ConnectClient", parameters);
        }
        public LoginResponse Login(System.String userName, System.String passHash)
        {
            object[] parameters = new object[] { userName, passHash };
            return InvokeServiceMethod<LoginResponse>("Login", parameters);
        }
        public SignOutResponse EndSession()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<SignOutResponse>("EndSession", parameters);
        }
        public String WhoAmI()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<String>("WhoAmI", parameters);
        }
    }

}
namespace Bam.Net.CoreServices.Contracts
{
	using System;
	using Bam.Net.Configuration;
	using Bam.Net.ServiceProxy;
	using Bam.Net.ServiceProxy.Encryption;
	using Bam.Net.CoreServices.Contracts;
	using Bam.Net.CoreServices;
	using Bam.Net.CoreServices.ApplicationRegistration.Data;
	using System.Collections.Generic;
	using Bam.Net.UserAccounts;

    
        public interface IApplicationRegistryService
        {
			ApiSigningKeyInfo[] ListApiKeys();
			ApiSigningKeyInfo AddApiKey();
			ApiSigningKeyInfo SetActiveApiKeyIndex(System.Int32 index);
			String GetApplicationName();
			ApiSigningKeyInfo GetClientApiKeyInfo();
			CoreServiceResponse RegisterApplication(System.String applicationName);
			CoreServiceResponse RegisterApplicationProcess(Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor descriptor);
			CoreServiceResponse RegisterClient(Bam.Net.CoreServices.ApplicationRegistration.Data.Client client);
			Dictionary<System.String, System.String> GetSettings();
			LoginResponse ConnectClient(Bam.Net.CoreServices.ApplicationRegistration.Data.Client client);
			LoginResponse Login(System.String userName, System.String passHash);
			SignOutResponse EndSession();
			String WhoAmI();

        }

}
/*
This file was generated and should not be modified directly
*/

namespace Bam.Net.CoreServices
{
    using System;
    using Bam.Net;
    using Bam.Net.ServiceProxy;
    using Bam.Net.ServiceProxy.Encryption;
    using Bam.Net.CoreServices.Contracts;
	using Bam.Net.ServiceProxy.Encryption;
	using System;
	using Bam.Net.CoreServices;
	using System.Collections.Generic;
	using Bam.Net.UserAccounts;

	public class ApplicationRegistryServiceProxy: ApplicationRegistryService, IProxy 
	{
		ApplicationRegistryServiceClient _proxyClient;
		public ApplicationRegistryServiceProxy()
		{
			_proxyClient = new ApplicationRegistryServiceClient();
		}

		public ApplicationRegistryServiceProxy(string baseUrl)
		{
			_proxyClient = new ApplicationRegistryServiceClient(baseUrl);
		}

		public ServiceProxyClient Client
		{
			get
			{
				return _proxyClient;
			}		
		}

		public Type ProxiedType
		{
			get
			{
				return typeof(ApplicationRegistryService);
			}
		}

		public IApiSigningKeyResolver ApiKeyResolver 
		{
			get
			{
				return (IApiSigningKeyResolver)_proxyClient.Property("ApiKeyResolver", false);
			}
			set
			{
				_proxyClient.Property("ApiKeyResolver", value, false);
			}
		}

		public IApplicationNameProvider ClientApplicationNameProvider
		{
			get
			{
				return (IApplicationNameProvider)_proxyClient.Property("ClientApplicationNameProvider", false);
			}
			set
			{
				_proxyClient.Property("ClientApplicationNameProvider", value, false);
			}
		}

		public void SubscribeToClientEvent(string eventName, EventHandler handler)
		{
			_proxyClient.Subscribe(eventName, handler);
		}


/*		public override ApiKeyInfo[] ListApiKeys()
		{
			return _proxyClient.ListApiKeys();
		}*/

		public override ApiSigningKeyInfo AddApiKey()
		{
			return _proxyClient.AddApiKey();
		}

		public override ApiSigningKeyInfo SetActiveApiKeyIndex(System.Int32 index)
		{
			return _proxyClient.SetActiveApiKeyIndex(index);
		}

		public override String GetApplicationName()
		{
			return _proxyClient.GetApplicationName();
		}

		public override ApiSigningKeyInfo GetClientApiKeyInfo()
		{
			return _proxyClient.GetClientApiKeyInfo();
		}

		public override CoreServiceResponse RegisterApplication(System.String applicationName)
		{
			return _proxyClient.RegisterApplication(applicationName);
		}

		public override CoreServiceResponse RegisterApplicationProcess(Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor descriptor)
		{
			return _proxyClient.RegisterApplicationProcess(descriptor);
		}

		public override CoreServiceResponse RegisterClient(Bam.Net.CoreServices.ApplicationRegistration.Data.Client client)
		{
			return _proxyClient.RegisterClient(client);
		}

		public override Dictionary<System.String, System.String> GetSettings()
		{
			return _proxyClient.GetSettings();
		}

		public override LoginResponse ConnectClient(Bam.Net.CoreServices.ApplicationRegistration.Data.Client client)
		{
			return _proxyClient.ConnectClient(client);
		}

		public override LoginResponse Login(System.String userName, System.String passHash)
		{
			return _proxyClient.Login(userName, passHash);
		}

		public override SignOutResponse EndSession()
		{
			return _proxyClient.EndSession();
		}

		public override String WhoAmI()
		{
			return _proxyClient.WhoAmI();
		}
	}
}																								

