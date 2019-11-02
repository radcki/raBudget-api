using System.Collections.Generic;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers.ListClosestOccurrences
{
    public class ListClosestOccurrencesRequest : IRequest<IEnumerable<TransactionDto>>
    {
        public BudgetDto Budget { get; set; }
        public int DaysForwardLimit { get; set; }
        public int DaysBackwardLimit { get; set; }

        public ListClosestOccurrencesRequest(BudgetDto budget, int daysBackwardLimit, int daysForwardLimit)
        {
            Budget = budget;
            DaysForwardLimit = daysForwardLimit;
            DaysBackwardLimit = daysBackwardLimit;
        }
    }

    public class GetTransactionScheduleRequestValidator : AbstractValidator<ListClosestOccurrencesRequest>
    {
        public GetTransactionScheduleRequestValidator()
        {
            RuleFor(x => x.Budget).NotEmpty();
            RuleFor(x => x.Budget.BudgetId).NotEmpty();
        }
    }


}
