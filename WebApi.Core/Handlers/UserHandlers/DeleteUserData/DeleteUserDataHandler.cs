using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.UserHandlers.DeleteUserData
{
    public class DeleteUserDataHandler : IRequestHandler<DeleteUserDataRequest, DeleteUserDataResponse>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly ILogger<DeleteUserDataHandler> _logger;

        public DeleteUserDataHandler(IUserRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider, ILogger<DeleteUserDataHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<DeleteUserDataResponse> Handle(DeleteUserDataRequest request, CancellationToken cancellationToken)
        {
            // only admin can delete other users
            if (_authenticationProvider.User.UserId != request.UserToDelete.UserId && !_authenticationProvider.Principal.IsInRole("admin"))
            {
                return new DeleteUserDataResponse() {ResponseType = eResponseType.Unauthorized};
            }

            var userEntity = await _repository.GetByIdAsync(request.UserToDelete.UserId);
            if (userEntity == null)
            {
                throw new DeleteFailureException(nameof(userEntity), userEntity, "User not found");
            }

            await _repository.DeleteAsync(userEntity);
            return new DeleteUserDataResponse(){ResponseType = eResponseType.Success};
        }
    }
}