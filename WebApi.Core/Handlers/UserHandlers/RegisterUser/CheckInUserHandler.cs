using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.UserHandlers.RegisterUser
{
    public class CheckInUserHandler : IRequestHandler<CheckInUserRequest, UserDto>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly ILogger<CheckInUserHandler> _logger;

        public CheckInUserHandler(IUserRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider, ILogger<CheckInUserHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<UserDto> Handle(CheckInUserRequest request, CancellationToken cancellationToken)
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