namespace Bam.Net.CoreServices.AccessControl
{
    
    public class TokenAuthorizationHeader : AuthorizationHeader
    {
        private const string ValuePrefix = "token ";
        public TokenAuthorizationHeader()
        {
        }

        private string _value;
        public override string Value
        {
            get => _value;
            set => _value = value == null ? value : value.StartsWith(ValuePrefix) ? value : $"{ValuePrefix} {value}";
        }
    }
}