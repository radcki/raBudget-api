using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Features.User.Command
{
    public class CheckInUser
    {
        /// <summary>
        /// Request is empty as user data is taken from IAuthenticationProvider
        /// </summary>
        public class Command : IRequest<UserDto>
        {
        }

        public class Handler : IRequestHandler<Command, UserDto>
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
            public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingUser = await _repository.GetByIdAsync(_authenticationProvider.User.UserId);
                if (existingUser.IsNullOrDefault())
                {
                    var mappedUser = _mapper.Map<Domain.Entities.User>(_authenticationProvider.User);
                    mappedUser.CreationTime = DateTime.Now;

                    existingUser = await _repository.AddAsync(mappedUser);
                    var result = await _repository.SaveChangesAsync(cancellationToken);
                }

                return _mapper.Map<UserDto>(existingUser);
            }
        }
    }
}