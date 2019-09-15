using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using raBudget.Core.Dto.User;
using raBudget.Core.ExtensionMethods;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.UserHandlers.RegisterUser
{
    public class CheckInUserHandler : IRequestHandler<CheckInUserRequest, CheckInUserResponse>
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
        public async Task<CheckInUserResponse> Handle(CheckInUserRequest request, CancellationToken cancellationToken)
        {
            var existingUser = await _repository.GetByIdAsync(_authenticationProvider.User.UserId);
            if (existingUser.IsNullOrDefault())
            {
                var mappedUser = _mapper.Map<Domain.Entities.User>(_authenticationProvider.User);

                existingUser = await _repository.AddAsync(mappedUser);
                try
                {
                    _logger.LogInformation("Saving user {@User} to repository: {@Request}", _authenticationProvider.User, request);
                    var result = await _repository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception when saving user {@User} to repository: {@Request}", mappedUser, request);
                    return new CheckInUserResponse()
                           {
                               ResponseType = eResponseType.Error
                           };
                }

            }

            return new CheckInUserResponse()
                   {
                       Data = _mapper.Map<UserDto>(existingUser),
                       ResponseType = eResponseType.Success
                   };
        }
    }
}