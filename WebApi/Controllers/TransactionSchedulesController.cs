using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Handlers.TransactionSchedule.Command;
using raBudget.Core.Handlers.TransactionSchedule.Query;

namespace raBudget.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/budgets/{budgetId}/[controller]")]
    public class TransactionSchedulesController : BaseController
    {
        #region TransactionSchedules CRUD

        /// <summary>
        /// Get list of transactionSchedules available for user - both owned and shared
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get([FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new ListTransactionSchedules.Query(new BudgetDto() {BudgetId = budgetId}, null));
            return Ok(response);
        }

        /// <summary>
        ///  Get details of specific transactionSchedule, identified by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetTransactionSchedule.Query(id));
            return Ok(response);
        }

        [HttpPost("filter")]
        public async Task<ActionResult> GetFiltered([FromBody] TransactionScheduleFilterDto filters, [FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new ListTransactionSchedules.Query(new BudgetDto() {BudgetId = budgetId}, filters)
                                               {
                                                   Filters = filters
                                               });
            return Ok(response);
        }

        /// <summary>
        /// Create new transactionSchedule
        /// </summary>
        /// <param name="transactionScheduleDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] TransactionScheduleDto transactionScheduleDto)
        {
            var response = await Mediator.Send(new CreateTransactionSchedule.Command(transactionScheduleDto));
            return Ok(response);
        }

        /// <summary>
        /// Update transactionSchedule parameters. TransactionSchedule id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] TransactionScheduleDetailsDto transactionScheduleDto, [FromRoute] int id)
        {
            transactionScheduleDto.TransactionScheduleId = id;
            var response = await Mediator.Send(new UpdateTransactionSchedule.Command(transactionScheduleDto));
            return Ok(response);
        }

        /// <summary>
        /// Delete transactionSchedule
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteTransactionSchedule.Command(id));
            return Ok(response);
        }

        #endregion

        [HttpGet("closest-transactions")]
        public async Task<ActionResult> ListClosestOccurrences(DateTime endDate, int budgetId)
        {
            var daysForward = (endDate - DateTime.Today).Days;
            var response = await Mediator.Send(new ListClosestOccurrences.Query(new BudgetDto() {BudgetId = budgetId}, 4, daysForward));
            return Ok(response);
        }
    }
}