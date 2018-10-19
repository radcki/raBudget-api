using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Enum;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController
    {
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public UsersController(UserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserDto userDto)
        {
            var user = _userService.Authenticate(userDto.Username, userDto.Password);

            if (user == null) return NotFound(new {message = "account.creditentialsInvalid"});

            string clientId = null;
            var randomNumber = new byte[8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                clientId = Convert.ToBase64String(randomNumber);
            }

            var tokenString = _userService.GenerateAccessToken(user);
            var refreshToken = _userService.GenerateRefreshToken(user, clientId);

            // return basic user info (without password) and token to store client side
            return Ok(new
                      {
                          User = new
                                 {
                                     Id = user.UserId,
                                     user.Username,
                                     Roles = user.UserRoles.Select(x => x.Role).ToList()
                                 },
                          Token = tokenString,
                          RefreshToken = refreshToken,
                          ClientId = clientId
                      });
        }

        [HttpPost("renewtoken")]
        [AllowAnonymous]
        public IActionResult RenewToken([FromBody] TokenRenewRequestDto requestData)
        {
            if (!_userService.ValidateRefreshToken(requestData.Token, requestData.RefreshToken, requestData.ClientId))
                return BadRequest(new {response = "invalid_token"});
            var principal = _userService.GetPrincipalFromExpiredToken(requestData.Token);

            var newJwtToken = _userService.GenerateAccessToken(principal);
            var newRefreshToken = _userService.GenerateRefreshToken(principal, requestData.ClientId);

            return Ok(new
                      {
                          token = newJwtToken,
                          refreshToken = newRefreshToken
                      });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userDto);

            try
            {
                // save 
                _userService.Create(user, userDto.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var userDtos = users.Select(x => new UserDto()
                                             {
                                                 Id = x.UserId,
                                                 Email = x.Email,
                                                 Username = x.Username
                                             })
                                .ToList();
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpGet("profile")]
        public IActionResult GetLoggedProfile(int id)
        {
            if (User != null)
                try
                {
                    var profile = _userService.GetById(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
                    var profileDto = new UserDto
                                     {
                                         Id = profile.UserId,
                                         Email = profile.Email,
                                         Username = profile.Username,
                                         DefaultBudgetId = profile.DefaultBudgetId
                                     };
                    return Ok(profileDto);
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpPut("updateprofile")]
        public IActionResult UpdateLogged([FromBody] UserDto userDto)
        {
            try
            {
                var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                // save 
                _userService.Update(new User
                                    {
                                        UserId = id,
                                        Email = userDto.Email
                                    });
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPut("changepassword")]
        public IActionResult PasswordChange([FromBody] PasswordChangeDto passwordChangeDto)
        {
            try
            {
                var username = User.Identity.Name;

                var user = _userService.Authenticate(username, passwordChangeDto.OldPassword);
                if (user == null) return BadRequest(new {message = "account.passwordInvalid"});
                if (passwordChangeDto.NewPassword.Length < 5)
                    return BadRequest(new {message = "forms.tooShortPassword"});
                _userService.ChangePassword(user, passwordChangeDto.NewPassword);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromBody] PasswordDto password)
        {
            var username = User.Identity.Name;

            var user = _userService.Authenticate(username, password.Password);
            if (user == null) return BadRequest(new { message = "account.passwordInvalid" });
            if (user.UserId == id || user.UserRoles.Select(x => x.Role).Contains(eRole.Admin))
            {
                _userService.Delete(id);
            }
            return Ok();
        }
    }
}