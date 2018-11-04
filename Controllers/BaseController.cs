using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        protected DataContext DatabaseContext { get; set; }

        private User _currentUser { get; set; }

        protected User CurrentUser
        {
            get
            {
                if (_currentUser != null) return _currentUser;
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var userEntity = DatabaseContext.Users.Single(x => x.UserId == userId);
                _currentUser = userEntity;
                return _currentUser;
            }
        }
    }
}