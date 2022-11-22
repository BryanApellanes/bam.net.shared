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
    /// A Serializable representation of an application signing key.
    /// </summary>
    public class ApiHmacKeyInfo
    {
        public ApiHmacKeyInfo()
        {
            this.ApplicationNameProvider = new DefaultConfigurationApplicationNameProvider();
        }

        public ApiHmacKeyInfo(IApplicationNameProvider nameProvider)
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
        public string ApiHmacKey
        {
            get;
            set;
        }

        public Bam.Net.CoreServices.ApplicationRegistration.Data.ApiHmacKey ToApiSigningKey()
        {
            Bam.Net.CoreServices.ApplicationRegistration.Data.ApiHmacKey key = new Bam.Net.CoreServices.ApplicationRegistration.Data.ApiHmacKey()
            {
                ClientIdentifier = this.ApplicationClientId,
                SharedSecret = this.ApiHmacKey
            };
            return key;
        }
    }
}
