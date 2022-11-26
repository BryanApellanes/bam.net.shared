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
        
        
		[ApiHmacKeyRequired]
        public ApiHmacKeyInfo[] ListApiKeys()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<ApiHmacKeyInfo[]>("ListApiKeys", parameters);
        }
		[ApiHmacKeyRequired]
        public ApiHmacKeyInfo AddApiKey()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<ApiHmacKeyInfo>("AddApiKey", parameters);
        }
		[ApiHmacKeyRequired]
        public ApiHmacKeyInfo SetActiveApiKeyIndex(System.Int32 index)
        {
            object[] parameters = new object[] { index };
            return InvokeServiceMethod<ApiHmacKeyInfo>("SetActiveApiKeyIndex", parameters);
        }
        public String GetApplicationName()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<String>("GetApplicationName", parameters);
        }
        public ApiHmacKeyInfo GetClientApiKeyInfo()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<ApiHmacKeyInfo>("GetClientApiKeyInfo", parameters);
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
			ApiHmacKeyInfo[] ListApiKeys();
			ApiHmacKeyInfo AddApiKey();
			ApiHmacKeyInfo SetActiveApiKeyIndex(System.Int32 index);
			String GetApplicationName();
			ApiHmacKeyInfo GetClientApiKeyInfo();
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

        public IApiHmacKeyResolver ApiHmacKeyResolver
        {
            get
            {
                return (IApiHmacKeyResolver)_proxyClient.Property("ApiHmacKeyResolver", false);
            }
            set
            {
                _proxyClient.Property("ApiHmacKeyResolver", value, false);
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

		public override ApiHmacKeyInfo AddApiKey()
		{
			return _proxyClient.AddApiKey();
		}

		public override ApiHmacKeyInfo SetActiveApiKeyIndex(System.Int32 index)
		{
			return _proxyClient.SetActiveApiKeyIndex(index);
		}

		public override String GetApplicationName()
		{
			return _proxyClient.GetApplicationName();
		}

		public override ApiHmacKeyInfo GetClientApiKeyInfo()
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

