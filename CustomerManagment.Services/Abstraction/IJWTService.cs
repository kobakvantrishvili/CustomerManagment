using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerManagment.Services.Abstraction
{
    public interface IJWTService
    {
        string GenerateSecurityToken(string Email);
    }
}
