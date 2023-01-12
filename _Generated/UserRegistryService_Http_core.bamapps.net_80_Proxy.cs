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
	using Bam.Net.UserAccounts;
	using System.Collections.Generic;
	using Bam.Net.CoreServices.ApplicationRegistration.Data;

    
    public class UserRegistryServiceClient: EncryptedServiceProxyClient<Bam.Net.CoreServices.Contracts.IUserRegistryService>, Bam.Net.CoreServices.Contracts.IUserRegistryService
    {
        public UserRegistryServiceClient(): base(DefaultConfiguration.GetAppSetting("UserRegistryServiceUrl", "http://core.bamapps.net/"))
        {
        }

        public UserRegistryServiceClient(string baseAddress): base(baseAddress)
        {
        }
        
        
        public ConfirmResponse ConfirmAccount(System.String token)
        {
            object[] parameters = new object[] { token };
            return InvokeServiceMethod<ConfirmResponse>("ConfirmAccount", parameters);
        }
        public ForgotPasswordResponse ForgotPassword(System.String emailAddress)
        {
            object[] parameters = new object[] { emailAddress };
            return InvokeServiceMethod<ForgotPasswordResponse>("ForgotPassword", parameters);
        }
        public CheckEmailResponse IsEmailInUse(System.String emailAddress)
        {
            object[] parameters = new object[] { emailAddress };
            return InvokeServiceMethod<CheckEmailResponse>("IsEmailInUse", parameters);
        }
        public CheckUserNameResponse IsUserNameAvailable(System.String userName)
        {
            object[] parameters = new object[] { userName };
            return InvokeServiceMethod<CheckUserNameResponse>("IsUserNameAvailable", parameters);
        }
        public SendEmailResponse RequestConfirmationEmail(System.String emailAddress, System.Int32 accountIndex)
        {
            object[] parameters = new object[] { emailAddress, accountIndex };
            return InvokeServiceMethod<SendEmailResponse>("RequestConfirmationEmail", parameters);
        }
        public PasswordResetResponse ResetPassword(System.String passHash, System.String resetToken)
        {
            object[] parameters = new object[] { passHash, resetToken };
            return InvokeServiceMethod<PasswordResetResponse>("ResetPassword", parameters);
        }
        public SignOutResponse SignOut()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<SignOutResponse>("SignOut", parameters);
        }
        public SignUpResponse SignUp(System.String emailAddress, System.String userName, System.String passHash, System.Boolean sendConfirmationEmail)
        {
            object[] parameters = new object[] { emailAddress, userName, passHash, sendConfirmationEmail };
            return InvokeServiceMethod<SignUpResponse>("SignUp", parameters);
        }
        public String GetCurrentUser()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<String>("GetCurrentUser", parameters);
        }
        public Boolean IsInRole(System.String roleName)
        {
            object[] parameters = new object[] { roleName };
            return InvokeServiceMethod<Boolean>("IsInRole", parameters);
        }
        public String[] GetRoles()
        {
            object[] parameters = new object[] {  };
            return InvokeServiceMethod<String[]>("GetRoles", parameters);
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
	using Bam.Net.UserAccounts;
	using System.Collections.Generic;
	using Bam.Net.CoreServices.ApplicationRegistration.Data;

    
        public interface IUserRegistryService
        {
			ConfirmResponse ConfirmAccount(System.String token);
			ForgotPasswordResponse ForgotPassword(System.String emailAddress);
			CheckEmailResponse IsEmailInUse(System.String emailAddress);
			CheckUserNameResponse IsUserNameAvailable(System.String userName);
			SendEmailResponse RequestConfirmationEmail(System.String emailAddress, System.Int32 accountIndex);
			PasswordResetResponse ResetPassword(System.String passHash, System.String resetToken);
			SignOutResponse SignOut();
			SignUpResponse SignUp(System.String emailAddress, System.String userName, System.String passHash, System.Boolean sendConfirmationEmail);
			String GetCurrentUser();
			Boolean IsInRole(System.String roleName);
			String[] GetRoles();
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
	using Bam.Net.UserAccounts;
	using System;
	using System.Collections.Generic;

	public class UserRegistryServiceProxy: UserRegistryService, IProxy 
	{
		UserRegistryServiceClient _proxyClient;
		public UserRegistryServiceProxy()
		{
			_proxyClient = new UserRegistryServiceClient();
		}

		public UserRegistryServiceProxy(string baseUrl)
		{
			_proxyClient = new UserRegistryServiceClient(baseUrl);
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
				return typeof(UserRegistryService);
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


		public override ConfirmResponse ConfirmAccount(System.String token)
		{
			return _proxyClient.ConfirmAccount(token);
		}

		public override ForgotPasswordResponse ForgotPassword(System.String emailAddress)
		{
			return _proxyClient.ForgotPassword(emailAddress);
		}

		public override CheckEmailResponse IsEmailInUse(System.String emailAddress)
		{
			return _proxyClient.IsEmailInUse(emailAddress);
		}

		public override CheckUserNameResponse IsUserNameAvailable(System.String userName)
		{
			return _proxyClient.IsUserNameAvailable(userName);
		}

		public override SendEmailResponse RequestConfirmationEmail(System.String emailAddress, System.Int32 accountIndex)
		{
			return _proxyClient.RequestConfirmationEmail(emailAddress, accountIndex);
		}

		public override PasswordResetResponse ResetPassword(System.String passHash, System.String resetToken)
		{
			return _proxyClient.ResetPassword(passHash, resetToken);
		}

		public override SignOutResponse SignOut()
		{
			return _proxyClient.SignOut();
		}

		public override SignUpResponse SignUp(System.String emailAddress, System.String userName, System.String passHash, System.Boolean sendConfirmationEmail)
		{
			return _proxyClient.SignUp(emailAddress, userName, passHash, sendConfirmationEmail);
		}

		public override String GetCurrentUser()
		{
			return _proxyClient.GetCurrentUser();
		}

		public override Boolean IsInRole(System.String roleName)
		{
			return _proxyClient.IsInRole(roleName);
		}

		public override String[] GetRoles()
		{
			return _proxyClient.GetRoles();
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

