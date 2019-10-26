using System.Collections.Generic;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers.ListTransactionSchedules
{
    public class ListTransactionSchedulesRequest : IRequest<IEnumerable<TransactionScheduleDto>>
    {
        public BudgetDto Budget { get; set; }
        public TransactionScheduleFilterDto Filters { get; set; }

        public ListTransactionSchedulesRequest(BudgetDto budget, TransactionScheduleFilterDto transactionScheduleFilter)
        {
            Budget = budget;
            Filters = transactionScheduleFilter;
        }
    }

    public class GetTransactionScheduleRequestValidator : AbstractValidator<ListTransactionSchedulesRequest>
    {
        public GetTransactionScheduleRequestValidator()
        {
            RuleFor(x => x.Budget).NotEmpty();
            RuleFor(x => x.Budget.BudgetId).NotEmpty();
        }
    }


}
