using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.CoreServices.AccessControl
{
    public class EnvironmentVariableAuthorizationHeaderProvider : AuthorizationHeaderProvider
    {
        public const string DefaultEnvironmentVariableName = "AuthorizationHeaderValue";

        public EnvironmentVariableAuthorizationHeaderProvider(): this(DefaultEnvironmentVariableName)
        {
        }

        public EnvironmentVariableAuthorizationHeaderProvider(string environmentVariableName)
        {
            this.TokenType = TokenTypes.Raw;
            this.EnvironmentVariableName = environmentVariableName;
        }
        
        public override string Value
        {
            get
            {
                return Environment.GetEnvironmentVariable(ConfigKey);
            }
            set
            {
                throw new InvalidOperationException($"This {nameof(AuthorizationHeaderProvider)} reads the value from the environment");
            }
        }

        public override string ConfigKey 
        {
            get;
            set;
        }

        public virtual string EnvironmentVariableName
        {
            get => ConfigKey;
            set => ConfigKey = value;
        }
    }
}
