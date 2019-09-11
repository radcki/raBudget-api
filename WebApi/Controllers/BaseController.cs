using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contexts;
using WebApi.Helpers;
using WebApi.Models.Entities;

namespace WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        protected DataContext DatabaseContext { get; set; }

    }
}