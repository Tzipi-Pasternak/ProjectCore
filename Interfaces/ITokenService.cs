using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ProjectCore.Interfaces
{
    public interface ITokenService
    { 
        public SecurityToken GetToken(List<Claim> claims);

        public TokenValidationParameters GetTokenValidationParameters();

        public string WriteToken(SecurityToken token);
    }
}