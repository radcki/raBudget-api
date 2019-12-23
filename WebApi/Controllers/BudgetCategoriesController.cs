using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.CreateBudgetCategory;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.DeleteBudgetCategory;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.GetBudgetCategory;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.ListBudgetCategories;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.UpdateBudgetCategory;

namespace WebApi.Controllers
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
            var response = await Mediator.Send(new ListBudgetCategoriesRequest(budgetId));
            return Ok(response);
        }

        /// <summary>
        ///  Get details of specific budget category, identified by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BudgetCategoryDto>> GetById([FromRoute] int id, [FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new GetBudgetCategoryRequest(id, budgetId));
            return Ok(response);
        }

        /// <summary>
        /// Create new budget
        /// </summary>
        /// <param name="budgetCategoryDto"></param>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BudgetCategoryDto>> Create([FromBody] BudgetCategoryDto budgetCategoryDto, [FromRoute] int budgetId)
        {
            budgetCategoryDto.BudgetId = budgetId;
            var response = await Mediator.Send(new CreateBudgetCategoryRequest(budgetCategoryDto));
            return Ok();
        }

        /// <summary>
        /// Update budget parameters. Budget id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BudgetCategoryDto>> Update([FromBody] BudgetCategoryDto budgetCategoryDto, [FromRoute] int budgetId)
        {
            budgetCategoryDto.BudgetId = budgetId;

            var response = await Mediator.Send(new UpdateBudgetCategoryRequest(budgetCategoryDto));
            return Ok(response);
        }

        /// <summary>
        /// Update budget parameters. Budget id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, int budgetId)
        {
            var response = await Mediator.Send(new DeleteBudgetCategoryRequest(id));
            return Ok(response);
        }
    }
}