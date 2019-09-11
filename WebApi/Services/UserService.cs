using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApi.Contexts;
using WebApi.Extensions;
using WebApi.Helpers;
using WebApi.Models.Dtos;
using WebApi.Models.Entities;
using WebApi.Models.Enum;

namespace WebApi.Services
{
    public class UserService
    {
        private readonly DataContext DatabaseContext;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;


        public UserService(DataContext context, ILoggerFactory loggerFactory)
        {
            DatabaseContext = context;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger("UserService");
        }

        public BaseResult<User> GetById(Guid userId)
        {
            var user = DatabaseContext.Users.Find(userId);
            return new BaseResult<User>()
                   {
                       Data = user,
                       Result = user != null ? eResultType.Success : eResultType.NotFound
                   };
        }

        public BaseResult<User> GetByClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.UserId();
            var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
            
            if (userId.IsNullOrDefault())
            {
                return new BaseResult<User>(){Result = eResultType.NotFound};
            }

            var user =  GetById(userId.Value).Data;

            /* Store new user */
            if (user == null)
            {
                user = new User()
                       {
                           UserId = userId.Value,
                           CreationTime = DateTime.Now,
                           Email = email
                       };
                DatabaseContext.Users.Add(user);
            }

            return new BaseResult<User>()
                   {
                       Data = user,
                       Result  = eResultType.Success
            };
        }

        public BaseResult DeleteUser(Guid userId)
        {
            var userEntity = GetById(userId).Data;
            if (userEntity == null)
            {
                return new BaseResult() { Result = eResultType.NotFound };
            }
            var userBudgets = DatabaseContext.Budgets.Where(x => x.UserId == userId);

            DatabaseContext.Budgets.RemoveRange(userBudgets);
            DatabaseContext.Users.Remove(userEntity);
            try
            {
                DatabaseContext.SaveChanges();
                return new BaseResult(){Result = eResultType.Success};
            }
            catch (Exception e)
            {
                _logger.LogError("DeleteUser exception",e);
                return new BaseResult() { Result = eResultType.Error };
            }
        }
    }
}