using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using raBudget.Core.Dto.User;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.User.Command
{
    public class DeleteUserData
    {
        public class Command : IRequest
        {
            public UserDto UserToDelete { get; set; }

            public Command(UserDto userToDelete)
            {
                UserToDelete = userToDelete;
            }
        }


        public class DeleteUserDataRequestValidator : AbstractValidator<Command>
        {
            public DeleteUserDataRequestValidator()
            {
                RuleFor(x => x.UserToDelete.UserId).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IUserRepository _repository;
            private readonly IMapper _mapper;
            private readonly IAuthenticationProvider _authenticationProvider;
            private readonly ILogger<Handler> _logger;

            public Handler(IUserRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider, ILogger<Handler> logger)
            {
                _repository = repository;
                _mapper = mapper;
                _authenticationProvider = authenticationProvider;
                _logger = logger;
            }

            /// <inheritdoc />
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                // only admin can delete other users
                if (_authenticationProvider.User.UserId != request.UserToDelete.UserId && !_authenticationProvider.Principal.IsInRole("admin"))
                {
                    throw new AuthenticationException();
                }

                var userEntity = await _repository.GetByIdAsync(request.UserToDelete.UserId);
                if (userEntity == null)
                {
                    throw new DeleteFailureException(nameof(userEntity), userEntity, "User not found");
                }

                await _repository.DeleteAsync(userEntity);
                return new Unit();
            }
        }
    }
}