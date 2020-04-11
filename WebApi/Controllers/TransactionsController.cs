using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Handlers.Transaction.Command;
using raBudget.Core.Handlers.Transaction.Query;

namespace raBudget.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/budgets/{budgetId}/[controller]")]
    public class TransactionsController : BaseController
    {
        #region Transactions CRUD

        /// <summary>
        ///  Get list of transactions from budget, filtered by query string
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get([FromRoute] int budgetId,[FromQuery] ListTransactions.Query query)
        {
            query.BudgetId = budgetId;
            var response = await Mediator.Send(query);
            return Ok(response.Data);
        }

        /// <summary>
        ///  Get details of specific transaction, identified by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{query.TransactionId}")]
        public async Task<ActionResult> GetById([FromQuery] GetTransaction.Query query)
        {
            var response = await Mediator.Send(query);
            return Ok(response.Data);
        }

        /// <summary>
        /// Create new transaction
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateTransaction.Request query)
        {
            var response = await Mediator.Send(query);
            return Ok(response);
        }

        /// <summary>
        /// Update transaction parameters. Transaction id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] UpdateTransaction.Request query, [FromRoute] int id)
        {
            query.TransactionId = id;
            var response = await Mediator.Send(query);
            return Ok(response);
        }

        /// <summary>
        /// Delete transaction
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteTransaction.Request(){TransactionId = id});
            return Ok(response);
        }

        #endregion
    }
}