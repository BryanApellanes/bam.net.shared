using System;
using System.Collections.Generic;
using System.Net;

namespace Bam.Net.CoreServices.AccessControl
{
    public abstract class AuthorizationHeaderProvider: IAuthorizationHeaderProvider
    {
        public AuthorizationHeaderProvider()
        {
            TokenType = TokenTypes.Bearer;

            Implementations = new Dictionary<TokenTypes, Func<AuthorizationHeader>>()
            {
                {TokenTypes.Raw, () => new AuthorizationHeader {Value = Value}},
                {TokenTypes.Bearer, () => new BearerTokenAuthorizationHeader {Value = Value}},
                {TokenTypes.Token, () => new TokenAuthorizationHeader {Value = Value}}
            };
        }
        
        public virtual TokenTypes TokenType { get; set; }
        
        public abstract string Value { get; set; }

        public virtual string GetRawValue()
        {
            return this.Value;
        }

        public AuthorizationHeader GetAuthorizationHeader(TokenTypes tokenType, string value)
        {
            TokenType = tokenType;
            return GetAuthorizationHeader(value);
        }

        public abstract string ConfigKey { get; set; }

        public AuthorizationHeader GetAuthorizationHeader(string value)
        {
            Value = value;
            return GetAuthorizationHeader();
        }
        
        public AuthorizationHeader GetAuthorizationHeader()
        {
            return Implementations[TokenType]();
        }

        public virtual AuthorizationHeader GetAuthorizationHeader(WebClient webClient)
        {
            return Implementations[TokenType]();
        }

        protected Dictionary<TokenTypes, Func<AuthorizationHeader>> Implementations
        {
            get;
            set;
        }
    }
}