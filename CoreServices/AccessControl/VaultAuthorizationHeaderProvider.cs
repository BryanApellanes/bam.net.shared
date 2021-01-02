using Bam.Net.Encryption;

namespace Bam.Net.CoreServices.AccessControl
{
    public class VaultAuthorizationHeaderProvider : AuthorizationHeaderProvider
    {
        public const string DefaultConfigKey = "AuthorizationHeader";
        
        public VaultAuthorizationHeaderProvider(): this(Vault.Profile)
        {
        }

        public VaultAuthorizationHeaderProvider(Vault vault)
        {
            ConfigKey = DefaultConfigKey;
            Vault = vault;
        }

        public VaultAuthorizationHeaderProvider(string value, Vault vault)
        {
            ConfigKey = DefaultConfigKey;
            Vault = vault;
            Value = value;
        }

        public VaultAuthorizationHeaderProvider(string key, string value, Vault vault)
        {
            ConfigKey = key;
            Vault = vault;
            Value = value;
        }
        
        protected Vault Vault { get; }
        
        public string ConfigKey { get; set; }

        private string _value;
        public override string Value
        {
            get => _value;
            set
            {
                _value = value;
                Vault[ConfigKey] = _value;
            }
        }
    }
}