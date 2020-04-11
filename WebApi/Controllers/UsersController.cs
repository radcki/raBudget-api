using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Dto.User;
using raBudget.Core.Handlers.User.Command;

namespace raBudget.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController
    {
        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete([FromRoute] Guid userId)
        {
            var response = await Mediator.Send(new DeleteUserData.Command(new UserDto() {UserId = userId}));
            return Ok(response);
        }
    }
}