using System;
using System.Collections.Generic;
using System.Text;
using LCW.Core.Entities.Concrete;

namespace LCW.Core.Utilities.Security.Jwt
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(User user, List<Role> operationClaims);
    }
}
