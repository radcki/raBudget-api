using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Handlers.BudgetHandlers.GetUnassignedFunds;
using raBudget.Core.Handlers.BudgetHandlers.Query;
using raBudget.Core.Handlers.UserHandlers.SetDefaultBudget;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using WebApi.Controllers;

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
            var response = await Mediator.Send(new GetBudget.Query(){BudgetId = id});
            return Ok(response.Data);
        }

        /// <summary>
        /// Create new budget
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateBudget.Request request)
        {
            var response = await Mediator.Send(request);
            return Ok(response.Data);
        }

        /// <summary>
        /// Update budget parameters. Budget id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] UpdateBudget.Request request, [FromRoute] int id)
        {
            request.BudgetId = id;
            var response = await Mediator.Send(request);
            return Ok(response.Data);
        }
        
        /// <summary>
        /// Delete budget with all related data
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteBudget.Request(){BudgetId = id});
            return Ok(response.Data);
        }
        
        /// <summary>
        /// Sets budget as default. Only one budget can be default and a time.
        /// </summary>
        /// <returns></returns>
        [HttpPatch("{id}/default")]
        public async Task<ActionResult> SetDefault([FromRoute] int id)
        {
            var response = await Mediator.Send(new SetDefaultBudgetRequest(id));
            return Ok(response);
        }

        #endregion

        #region Business queries

        [HttpGet("{budgetId}/categories-balance/{categoryType}")]
        public async Task<ActionResult> SpendingBalance([FromRoute] int budgetId, [FromRoute] eBudgetCategoryType categoryType)
        {
            var response = await Mediator.Send(new GetCategoryTypeBalanceRequest(budgetId, categoryType));
            return Ok(response);
        }

        [HttpGet("{budgetId}/unassigned-funds")]
        public async Task<ActionResult> UnassignedFunds(int budgetId)
        {
            var response = await Mediator.Send(new GetUnassignedFunds.Query() { BudgetId = budgetId});
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

            return Ok(Currency.CurrencyDictionary.Select(x=>x.Value));
        }
        
        #endregion
    }
}