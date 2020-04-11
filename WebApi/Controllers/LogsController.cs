using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace raBudget.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LogsController : BaseController
    {
        /*
        public LogsController(DataContext context)
        {
            DatabaseContext = context;
        }

        [HttpGet("all")]
        [Authorize("admin")]
        public IActionResult GetAll()
        {
            try
            {
                var logs = DatabaseContext.Logs.AsEnumerable().Select(x=>x.ToDto()).ToList();

                if (logs.Any())
                {
                    return Ok(logs);
                }
                else
                {
                    return NoDataFound();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("period")]
        [Authorize("admin")]
        public IActionResult GetPeriod(DateTime from, DateTime to)
        {
            try
            {
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
                    return NoDataFound();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        */
    }
}