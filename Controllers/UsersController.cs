﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Enum;
using WebApi.Helpers;
using WebApi.Services;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger Logger;

        public UsersController(UserService userService, IMapper mapper, DataContext context, ILoggerFactory loggerFactory)
        {
            _userService = userService;
            _mapper = mapper;
            DatabaseContext = context;
            _loggerFactory = loggerFactory;
            Logger = _loggerFactory.CreateLogger("UsersController");
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserDto userDto)
        {
            var result = _userService.Authenticate(userDto.Username, userDto.Password);

            if (result.Result != eResultType.Success)
            {
                return NotFound(new {message = "account.creditentialsInvalid"});
            }
            

            var user = result.Data;
            string clientId = null;
            var randomNumber = new byte[8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                clientId = Convert.ToBase64String(randomNumber);
            }

            var tokenString = _userService.GenerateAccessToken(user);
            var refreshToken = _userService.GenerateRefreshToken(user, clientId);

            // dane do zachowania w przeglądarce
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
            {
                return BadRequest(new {response = "invalid_token"});
            }

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
            if (!CurrentUser.UserRoles.Select(x => x.Role).Contains(eRole.Admin))
            {
                return Unauthorized();
            }

            var users = _userService.GetAll();
            var userDtos = users.Select(x => new UserDto()
                                             {
                                                 Id = x.UserId,
                                                 Email = x.Email,
                                                 Username = x.Username,
                                                 CreationDate = x.CreationTime,
                                                 Roles = x.UserRoles.Select(s => s.Role).ToList()
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

        [HttpPut("admin-updateprofile")]
        public IActionResult UpdateExternal([FromBody] UserDto userDto)
        {
            try
            {
                if (!CurrentUser.UserRoles.Select(x => x.Role).Contains(eRole.Admin))
                {
                    Logger.Log(LogLevel.Warning, "Admin request to update user " + userDto.Id + " invoked by user" + CurrentUser.UserId + " failed: user is not admin");
                    return Unauthorized();
                }

                var user = new User
                           {
                               UserId = userDto.Id,
                               Username = userDto.Username,
                               Email = userDto.Email
                           };

                _userService.Update(user);

                if (userDto.Roles != null)
                {
                    Logger.Log(LogLevel.Information, "Admin request to update user " + userDto.Id + " invoked by user" + CurrentUser.UserId + ": success");
                    _userService.UpdateRoles(user, userDto.Roles);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Admin request to update user " + userDto.Id + " invoked by user" + CurrentUser.UserId + " exception: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPut("changepassword")]
        public IActionResult PasswordChange([FromBody] PasswordChangeDto passwordChangeDto)
        {
            var username = User.Identity.Name;

            var authenticationResult = _userService.Authenticate(username, passwordChangeDto.OldPassword);

            if (authenticationResult.Result != eResultType.Success)
            {
                return BadRequest(new {message = "account.passwordInvalid"});
            }

            if (passwordChangeDto.NewPassword.Length < 5)
            {
                return BadRequest(new {message = "forms.tooShortPassword"});
            }

            var user = authenticationResult.Data;

            var operationResult = _userService.ChangePassword(user, passwordChangeDto.NewPassword);
            if (operationResult.Result == eResultType.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new {message = operationResult.Message});
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromBody] PasswordDto password)
        {
            var username = User.Identity.Name;

            var authenticationResult = _userService.Authenticate(username, password.Password);
            if (authenticationResult.Result != eResultType.Success)
            {
                return BadRequest(new { message = "account.passwordInvalid" });
            }

            var user = authenticationResult.Data;
            if (user.UserId != id && !user.UserRoles.Select(x => x.Role).Contains(eRole.Admin))
            {
                Logger.Log(LogLevel.Warning, "Request to delete user " + user.UserId + " interrupted: authorization failed");
                return Unauthorized();
            }
            
            var operationResult = _userService.Delete(id);
            if (operationResult.Result == eResultType.Success)
            {
                return Ok();
            }
            else
            {
                
                return BadRequest(new { message = operationResult.Message });
            }

        }
    }
}