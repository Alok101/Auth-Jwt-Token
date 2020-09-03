using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Auth.Demo.Models
{
    public interface IJwtAuthenticationManager
    {
        AuthenticationResponse Authenticate(string username, string password);
        IDictionary<string, string> UsersRefreshTokens { get; set; }
        AuthenticationResponse Authenticate(string username, Claim[] claims);
    }
}
