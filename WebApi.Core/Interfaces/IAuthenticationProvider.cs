using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using raBudget.Core.Dto.User;
using raBudget.Domain.Entities;

namespace raBudget.Core.Interfaces
{
    public interface IAuthenticationProvider
    {
        bool IsAuthenticated { get; }
        UserDto User { get; }
        ClaimsPrincipal Principal { get; }

        void FromAuthenticationResult(ClaimsPrincipal claimsPrincipal);
    }
}
