using System.Net;

namespace Bam.Net.CoreServices.AccessControl
{
    public interface IAuthorizationHeaderProvider
    {
        string ConfigKey { get; set; }
        AuthorizationHeader GetAuthorizationHeader(string value);
        AuthorizationHeader GetAuthorizationHeader(TokenTypes tokenType, string value);
        AuthorizationHeader GetAuthorizationHeader();
        AuthorizationHeader GetAuthorizationHeader(WebClient webClient);
    }
}