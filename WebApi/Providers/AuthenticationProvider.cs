using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces;

namespace raBudget.WebApi.Providers
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        #region Implementation of IAuthenticationProvider

        /// <inheritdoc />
        public bool IsAuthenticated { get; private set; }

        /// <inheritdoc />
        public UserDto User { get; private set; }

        /// <inheritdoc />
        public ClaimsPrincipal Principal { get; private set; }

        public AuthenticationProvider()
        {
            IsAuthenticated = false;
            User = null;
        }

  
        public void FromAuthenticationResult(ClaimsPrincipal claimsPrincipal)
        {
            Principal = claimsPrincipal;
            IsAuthenticated = claimsPrincipal.Identity.IsAuthenticated;
            User = IsAuthenticated
                           ? new UserDto()
                             {
                                 UserId = Guid.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value),
                                 Email = claimsPrincipal.FindFirst(ClaimTypes.Email).Value
                             }
                           : null;
        }

        #endregion
    }
}
