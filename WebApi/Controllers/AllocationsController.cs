using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Features.Allocation.Command;
using raBudget.Core.Features.Allocation.Query;

namespace raBudget.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/budgets/{budgetId}/[controller]")]
    public class AllocationsController : BaseController
    {
        #region Allocations CRUD

        /// <summary>
        /// Get list of allocations available for user - both owned and shared
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get([FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new ListAllocations.Query(new BudgetDto() {BudgetId = budgetId}));
            return Ok(response);
        }

        /// <summary>
        ///  Get details of specific allocation, identified by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetAllocation.Query(id));
            return Ok(response);
        }

        [HttpPost("filter")]
        public async Task<ActionResult> GetFiltered([FromBody] AllocationFilterDto filters, [FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new ListAllocations.Query(new BudgetDto() {BudgetId = budgetId})
                                               {
                                                   Filters = filters
                                               });
            return Ok(response);
        }

        /// <summary>
        /// Create new allocation
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateAllocation.Command command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Update allocation parameters. Allocation id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] UpdateAllocation.Command command, [FromRoute] int id)
        {
            command.AllocationId = id;
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Delete allocation
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteAllocation.Command(id));
            return Ok(response);
        }

        #endregion
    }
}