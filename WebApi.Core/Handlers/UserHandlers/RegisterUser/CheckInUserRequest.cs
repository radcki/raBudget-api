using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.User;

namespace raBudget.Core.Handlers.UserHandlers.RegisterUser
{
    /// <summary>
    /// Request is empty as user data is taken from IAuthenticationProvider
    /// </summary>
    public class CheckInUserRequest : IRequest<CheckInUserResponse>
    {
    }

    public class CheckInUserResponse : BaseResponse<UserDto>
    {
    }
}
