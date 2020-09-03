using Auth.Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Auth.Demo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        private readonly ITokenRefresher tokenRefresher;
        public TokenController(IJwtAuthenticationManager jwtAuthenticationManager, ITokenRefresher tokenRefresher)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this.tokenRefresher = tokenRefresher;
        }
        // GET: api/Token
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate([FromBody]UserCred userCred)
        {
           var token= jwtAuthenticationManager.Authenticate(userCred.UserName, userCred.Password);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }
        [AllowAnonymous]
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody]RefreshCred refreshCred)
        {
            var token = tokenRefresher.Refresh(refreshCred);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }

    }
}
