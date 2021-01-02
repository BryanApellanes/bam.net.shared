namespace Bam.Net.CoreServices.AccessControl
{
    public class BearerTokenAuthorizationHeader : AuthorizationHeader
    {
        private const string ValuePrefix = "bearer ";

        public BearerTokenAuthorizationHeader()
        {
        }
        
        private string _value;
        public override string Value
        {
            get => _value;
            set => _value = value == null ? "" : value.StartsWith(ValuePrefix) ? value : $"{ValuePrefix} {value}";
        }
    }
}