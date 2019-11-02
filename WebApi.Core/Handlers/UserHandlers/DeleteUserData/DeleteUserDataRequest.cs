using FluentValidation;
using MediatR;
using raBudget.Core.Dto.User;

namespace raBudget.Core.Handlers.UserHandlers.DeleteUserData
{
    /// <summary>
    /// Request is empty as user data is taken from IAuthenticationProvider
    /// </summary>
    public class DeleteUserDataRequest : IRequest
    {
        public UserDto UserToDelete { get; set; }

        public DeleteUserDataRequest(UserDto userToDelete)
        {
            UserToDelete = userToDelete;
        }
    }


    public class DeleteUserDataRequestValidator : AbstractValidator<DeleteUserDataRequest>
    {
        public DeleteUserDataRequestValidator()
        {
            RuleFor(x => x.UserToDelete.UserId).NotEmpty();
        }
    }
}
