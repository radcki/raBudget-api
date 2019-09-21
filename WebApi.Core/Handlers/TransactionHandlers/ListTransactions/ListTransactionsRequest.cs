using System.Collections.Generic;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;

namespace raBudget.Core.Handlers.TransactionHandlers.ListTransactions
{
    public class ListTransactionsRequest : IRequest<IEnumerable<TransactionDto>>
    {
        public BudgetDto Budget { get; set; }

        public ListTransactionsRequest(BudgetDto budget)
        {
            Budget = budget;
        }
    }

    public class GetTransactionRequestValidator : AbstractValidator<ListTransactionsRequest>
    {
        public GetTransactionRequestValidator()
        {
            RuleFor(x => x.Budget).NotEmpty();
            RuleFor(x => x.Budget.BudgetId).NotEmpty();
        }
    }


}
