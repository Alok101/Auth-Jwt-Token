using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Demo.Models
{
    public class TokenRefresher : ITokenRefresher
    {
        private readonly byte[] key;
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        public TokenRefresher(byte[] key,IJwtAuthenticationManager jwtAuthenticationManager)
        {
            this.key = key;
            this.jwtAuthenticationManager = jwtAuthenticationManager;
        }
        public AuthenticationResponse Refresh(RefreshCred refreshCred)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(refreshCred.JwtToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            if(jwtToken==null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token passed!");
            }
            var userName = principal.Identity.Name;
            if (refreshCred.RefreshToken != jwtAuthenticationManager.UsersRefreshTokens[userName])
            {
                throw new SecurityTokenException("Invalid token passed!");
            }
            return jwtAuthenticationManager.Authenticate(userName, principal.Claims.ToArray());
        }
    }
}
