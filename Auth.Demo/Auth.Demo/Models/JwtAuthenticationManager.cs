using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Demo.Models
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly IDictionary<string, string> users = new Dictionary<string, string>
        {{"test1","password1"},{"test2","password2"} };
        public IDictionary<string, string> UsersRefreshTokens { get; set; }
        private readonly string key;
        private readonly IRefreshTokenGenerator refreshTokenGenerator;
        public JwtAuthenticationManager(string key,IRefreshTokenGenerator refreshTokenGenerator)
        {
            this.key = key;
            this.refreshTokenGenerator = refreshTokenGenerator;
            UsersRefreshTokens = new Dictionary<string, string>();
        }
        public AuthenticationResponse Authenticate(string username, string password)
        {
            if (!users.Any(u => u.Key == username && u.Value == password))
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                { 
                    new Claim(ClaimTypes.Name, username) 
                }),
                Expires=DateTime.UtcNow.AddHours(1),
                SigningCredentials=new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),SecurityAlgorithms.HmacSha256Signature
                    )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = refreshTokenGenerator.GenerateToken();
            if (UsersRefreshTokens.ContainsKey(username))
            {
                UsersRefreshTokens[username] = refreshToken;
            }
            else
            {
                UsersRefreshTokens.Add(username, refreshToken);
            }
            return new AuthenticationResponse {JwtToken= tokenHandler.WriteToken(token),RefreshToken= refreshToken }; 
        }
        public AuthenticationResponse Authenticate(string username, Claim[] claims)
        {
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var jwtSecurityToken = new JwtSecurityToken(
                claims:claims,
                expires:DateTime.UtcNow.AddHours(1),
                signingCredentials:new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                    )
                );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var refreshToken = refreshTokenGenerator.GenerateToken();
            if (UsersRefreshTokens.ContainsKey(username))
            {
                UsersRefreshTokens[username] = refreshToken;
            }
            else
            {
                UsersRefreshTokens.Add(username, refreshToken);
            }
            return new AuthenticationResponse { JwtToken = token, RefreshToken = refreshToken };
        }

    }
}
