using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Features.Budget.Command;
using raBudget.Core.Features.Budget.Query;
using raBudget.Core.Features.BudgetCategories.Command;
using raBudget.Core.Features.BudgetCategories.Query;
using raBudget.Core.Features.User.Command;
using raBudget.Core.Interfaces;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using WebApi.Hubs;

namespace raBudget.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BudgetsController : BaseController
    {
        #region Budgets CRUD

        /// <summary>
        /// Get list of budgets available for user - both owned and shared
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] ListAvailableBudgets.Query query)
        {
            var response = await Mediator.Send(query);
            return Ok(response.Data);
        }

        /// <summary>
        ///  Get details of specific budget, identified by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetBudget.Query() {BudgetId = id});
            return Ok(response.Data);
        }

        /// <summary>
        /// Create new budget
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateBudget.Command command)
        {
            var response = await Mediator.Send(command);
            return Ok(response.Data);
        }

        /// <summary>
        /// Update budget parameters. Budget id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] UpdateBudget.Command command, [FromRoute] int id)
        {
            command.BudgetId = id;
            var response = await Mediator.Send(command);
            return Ok(response.Data);
        }

        /// <summary>
        /// Delete budget with all related data
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteBudget.Request() {BudgetId = id});
            return Ok(response.Data);
        }

        /// <summary>
        /// Sets budget as default. Only one budget can be default and a time.
        /// </summary>
        /// <returns></returns>
        [HttpPatch("{id}/default")]
        public async Task<ActionResult> SetDefault([FromRoute] int id)
        {
            var response = await Mediator.Send(new SetDefaultBudget.Command(id));
            return Ok(response);
        }

        /// <summary>
        /// Save order of budget categories
        /// </summary>
        /// <param name="command"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/save-categories-order")]
        public async Task<ActionResult> SaveBudgetCategoryOrder([FromBody] SaveBudgetCategoryOrder.Command command, [FromRoute] int id)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        #endregion

        #region Business queries

        [HttpGet("{budgetId}/categories-balance/{categoryType}")]
        public async Task<ActionResult> SpendingBalance([FromRoute] int budgetId, [FromRoute] eBudgetCategoryType categoryType)
        {
            var response = await Mediator.Send(new GetCategoryTypeBalance.Query(budgetId, categoryType));
            return Ok(response);
        }

        [HttpGet("{budgetId}/unassigned-funds")]
        public async Task<ActionResult> UnassignedFunds(int budgetId)
        {
            var response = await Mediator.Send(new GetUnassignedFunds.Query() {BudgetId = budgetId});
            return Ok(response.Data);
        }

        [HttpGet("{budgetId}/period-report")]
        public async Task<ActionResult> PeriodReport([FromRoute] int budgetId, [FromQuery] GetPeriodReport.Query query)
        {
            query.BudgetId = budgetId;
            var response = await Mediator.Send(query);
            return Ok(response.Data);
        }

        [HttpGet("{budgetId}/monthly-report")]
        public async Task<ActionResult> MonthlyReport([FromRoute] int budgetId, [FromQuery] GetMonthlyReport.Query query)
        {
            query.BudgetId = budgetId;
            var response = await Mediator.Send(query);
            return Ok(response.Data);
        }

        #endregion

        #region Utils

        [HttpGet("supported-currencies")]
        public ActionResult Currencies()
        {
            return Ok(Currency.CurrencyDictionary.Select(x => x.Value));
        }

        #endregion
    }
}