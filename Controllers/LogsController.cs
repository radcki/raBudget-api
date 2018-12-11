using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contexts;
using WebApi.Helpers;
using WebApi.Models.Enum;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LogsController : BaseController
    {
        public LogsController(DataContext context)
        {
            DatabaseContext = context;
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            try
            {
                if (!CurrentUser.UserRoles.Select(x => x.Role).Contains(eRole.Admin))
                {
                    return Unauthorized();
                }

                var logs = DatabaseContext.Logs.AsEnumerable().Select(x=>x.ToDto()).ToList();

                if (logs.Any())
                {
                    return Ok(logs);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("period")]
        public IActionResult GetPeriod(DateTime from, DateTime to)
        {
            try
            {
                if (!CurrentUser.UserRoles.Select(x => x.Role).Contains(eRole.Admin))
                {
                    return Unauthorized();
                }

                var logs = DatabaseContext.Logs
                                          .Where(x=>x.TimeStamp.Date >= from && x.TimeStamp.Date<=to)
                                          .AsEnumerable()
                                          .Select(x => x.ToDto()).ToList();

                if (logs.Any())
                {
                    return Ok(logs);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}