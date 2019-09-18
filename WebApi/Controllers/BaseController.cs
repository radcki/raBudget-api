using System;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces;
using raBudget.EfPersistence.Contexts;

namespace WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        #region Properties
        private IMediator _mediator;
        private IAuthenticationProvider _authenticationProvider;

        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
        protected IAuthenticationProvider AuthenticationProvider => _authenticationProvider ?? (_authenticationProvider = HttpContext.RequestServices.GetService<IAuthenticationProvider>());

        /// <summary>
        /// UserDto created from authenticated user ClaimsPrincipal
        /// </summary>
        protected UserDto TokenUserDto => new UserDto()
                                     {
                                         UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                                         Email = User.FindFirst(ClaimTypes.Email)?.Value,
                                     };

        #endregion
    }
}