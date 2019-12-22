using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Handlers.BudgetHandlers.CreateBudget;
using raBudget.Core.Handlers.BudgetHandlers.DeleteBudget;
using raBudget.Core.Handlers.BudgetHandlers.GetBudget;
using raBudget.Core.Handlers.BudgetHandlers.GetMonthlyReport;
using raBudget.Core.Handlers.BudgetHandlers.GetUnassignedFunds;
using raBudget.Core.Handlers.BudgetHandlers.ListAvailableBudgets;
using raBudget.Core.Handlers.BudgetHandlers.UpdateBudget;
using raBudget.Core.Handlers.UserHandlers.SetDefaultBudget;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace WebApi.Controllers
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
        public async Task<ActionResult> Get()
        {
            var response = await Mediator.Send(new ListAvailableBudgetsRequest());
            return Ok(response);
        }

        /// <summary>
        ///  Get details of specific budget, identified by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetBudgetRequest(id));
            return Ok(response);
        }

        /// <summary>
        /// Create new budget
        /// </summary>
        /// <param name="budgetDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] BudgetDto budgetDto)
        {
            budgetDto.OwnedByUser = AuthenticationProvider.User;
            var response = await Mediator.Send(new CreateBudgetRequest(budgetDto));
            return Ok(response);
        }

        /// <summary>
        /// Update budget parameters. Budget id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] BudgetDto budgetDto, [FromRoute] int id)
        {
            budgetDto.BudgetId = id;
            var response = await Mediator.Send(new UpdateBudgetRequest(budgetDto));
            return Ok(response);
        }
        
        /// <summary>
        /// Delete budget with all related data
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteBudgetRequest(id));
            return Ok(response);
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
            var response = await Mediator.Send(new GetUnassignedFundsRequest(budgetId));
            return Ok(response);
        }

        [HttpPost("{budgetId}/period-report")]
        public async Task<ActionResult> PeriodReport([FromRoute] int budgetId, [FromBody] ReportFilterDto filters)
        {
            var response = await Mediator.Send(new GetPeriodReportRequest(budgetId, filters));
            return Ok(response);
        }

        [HttpPost("{budgetId}/monthly-report")]
        public async Task<ActionResult> MonthlyReport([FromRoute] int budgetId, [FromBody] ReportFilterDto filters)
        {
            var response = await Mediator.Send(new GetMonthlyReportRequest(budgetId, filters));
            return Ok(response);
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