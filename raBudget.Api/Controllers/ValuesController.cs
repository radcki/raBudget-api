using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Dto.Budget.Request;
using raBudget.Core.Dto.Budget.Response;
using raBudget.Core.Dto.User;

namespace raBudget.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<ListAvailableBudgetsResponse>> Get()
        {
            return Ok(await Mediator.Send(new ListAvailableBudgetsRequest(new UserDto(){UserId = Guid.NewGuid()})));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
