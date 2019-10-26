using FluentValidation;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers.UpdateTransactionSchedule
{
    public class UpdateTransactionScheduleRequest : IRequest<TransactionScheduleDto>
    {
        public TransactionScheduleDetailsDto Data;

        public UpdateTransactionScheduleRequest(TransactionScheduleDetailsDto transactionSchedule)
        {
            Data = transactionSchedule;
        }
    }


    public class UpdateTransactionScheduleRequestValidator : AbstractValidator<UpdateTransactionScheduleRequest>
    {
        public UpdateTransactionScheduleRequestValidator()
        {
            RuleFor(x => x.Data.Description).NotEmpty();
            RuleFor(x => x.Data.BudgetCategoryId).NotEmpty();
            RuleFor(x => x.Data.Amount).NotEmpty();
        }
    }
    
}
