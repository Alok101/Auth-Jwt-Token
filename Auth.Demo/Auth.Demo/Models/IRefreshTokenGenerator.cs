using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Demo.Models
{
    public interface IRefreshTokenGenerator
    {
        string GenerateToken();
    }
}
