using System.Collections.Generic;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;

namespace raBudget.Core.Handlers.TransactionHandlers.ListTransactions
{
    public class ListTransactionsRequest : IRequest<ListTransactionsResponse>
    {
    }

    public class ListTransactionsResponse : BaseResponse<IEnumerable<TransactionDto>>
    {
    }

}
