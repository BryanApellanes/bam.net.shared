/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Configuration;

namespace Bam.Net.ServiceProxy.Encryption
{
    /// <summary>
    /// A Serializable representation of an applications
    /// credentials.
    /// </summary>
    public class ApiKeyInfo
    {
        public ApiKeyInfo()
        {
            this.ApplicationNameProvider = new DefaultConfigurationApplicationNameProvider();
        }
        public ApiKeyInfo(IApplicationNameProvider nameProvider)
        {
            ApplicationNameProvider = nameProvider;
        }
        protected internal IApplicationNameProvider ApplicationNameProvider
        {
            get;
            set;
        }
        string _appName;
        public string ApplicationName
        {
            get
            {
                if (string.IsNullOrEmpty(_appName))
                {
                    _appName = ApplicationNameProvider.GetApplicationName();
                }
                return _appName;
            }
            set
            {
                _appName = value;
            }
        }

        public string ApplicationClientId
        {
            get;
            set;
        }

        /// <summary>
        /// The shared secret; keep this value private.
        /// </summary>
        public string ApiKey
        {
            get;
            set;
        }

        public Bam.Net.CoreServices.ApplicationRegistration.Data.ApiKey ToApiKey()
        {
            Bam.Net.CoreServices.ApplicationRegistration.Data.ApiKey key = new Bam.Net.CoreServices.ApplicationRegistration.Data.ApiKey()
            {
                ClientIdentifier = this.ApplicationClientId,
                SharedSecret = this.ApiKey
            };
            return key;
        }

/*        public ApiKeyInfo ToKeyInfo()
        {
            return new ApiKeyInfo
            {
                ApplicationClientId = ClientIdentifier,
                ApiKey = SharedSecret,
                ApplicationNameProvider = new StaticApplicationNameProvider(Application.Name)
            };
        }*/
    }
}
