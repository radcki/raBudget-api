using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Features.BudgetCategories.Command;
using raBudget.Core.Features.BudgetCategories.Query;

namespace raBudget.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/budgets/{budgetId}/[controller]")]
    public class BudgetCategoriesController : BaseController
    {
        /// <summary>
        /// Get list of budget categories belonging to budget
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetCategoryDto>>> Get([FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new ListBudgetCategories.Query(budgetId));
            return Ok(response);
        }

        /// <summary>
        ///  Get details of specific budget category, identified by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BudgetCategoryDto>> GetById([FromRoute] int id, [FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new GetBudgetCategory.Query(id, budgetId));
            return Ok(response);
        }

        /// <summary>
        /// Create new budget
        /// </summary>
        /// <param name="budgetCategoryDto"></param>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BudgetCategoryDto>> Create([FromBody] CreateBudgetCategory.Command command, [FromRoute] int budgetId)
        {
            command.BudgetId = budgetId;
            var response = await Mediator.Send(command);
            return Ok();
        }

        /// <summary>
        /// Update budget parameters. Budget id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BudgetCategoryDto>> Update([FromBody] UpdateBudgetCategory.Command command, [FromRoute] int budgetId)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Update budget parameters. Budget id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, int budgetId)
        {
            var response = await Mediator.Send(new DeleteBudgetCategory.Command(id));
            return Ok(response);
        }
    }
}