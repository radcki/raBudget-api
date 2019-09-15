using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using raBudget.Core.Handlers.UserHandlers.RegisterUser;
using raBudget.Core.Interfaces;
using raBudget.Domain.Enum;

namespace raBudget.WebApi.Handlers
{
    /// <summary>
    /// Authorization pre-flight to ensure that external identity provider user is registered in application
    /// </summary>
    public class UserRegisteredHandler : AuthorizationHandler<DenyAnonymousAuthorizationRequirement>
    {
        /// <inheritdoc />
        public UserRegisteredHandler(IMediator mediator, IAuthenticationProvider authenticationProvider)
        {
            _mediator = mediator;
            _authenticationProvider = authenticationProvider;
        }
        private readonly IMediator _mediator;
        private readonly IAuthenticationProvider _authenticationProvider;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DenyAnonymousAuthorizationRequirement requirement)
        {
            if (_authenticationProvider.IsAuthenticated)
            {
                var result = _mediator.Send(new CheckInUserRequest()).Result;

                if (result.ResponseType != eResponseType.Success)
                {
                    throw new Exception("User registration failed");
                }
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
