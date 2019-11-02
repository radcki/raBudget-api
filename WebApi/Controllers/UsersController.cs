using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Dto.User;
using raBudget.Core.Handlers.UserHandlers.DeleteUserData;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController
    {
        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete([FromRoute] Guid userId)
        {
            var response = await Mediator.Send(new DeleteUserDataRequest(new UserDto(){UserId = userId}));
            return Ok(response);
        }
        /*
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        private readonly ILogger _logger;

        public UsersController(UserService userService, IMapper mapper, DataContext context, ILoggerFactory loggerFactory)
        {
            _userService = userService;
            _mapper = mapper;
            DatabaseContext = context;
            _logger = loggerFactory.CreateLogger("UsersController");
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var user = _userService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpGet("profile")]
        public IActionResult GetLoggedProfile()
        {
            if (User != null)
                try
                {
                    var profile = _userService.GetByClaimsPrincipal(User).Data;
                    var profileDto = profile.ToDto();

                    return Ok(profileDto);
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            var userId = User.UserId();
            if (!userId.IsNullOrDefault())
            {
                if (_userService.DeleteUser(userId.Value))
                    return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize("admin")]
        public IActionResult Delete(Guid id)
        {
            var operationResult = _userService.DeleteUser(id);
            if (operationResult.Result == eResultType.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new {message = operationResult.Message});
            }
        }
        */
    }
}