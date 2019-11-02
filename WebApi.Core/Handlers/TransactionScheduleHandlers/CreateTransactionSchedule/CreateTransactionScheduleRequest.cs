using FluentValidation;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers.CreateTransactionSchedule
{
    public class CreateTransactionScheduleRequest : IRequest<TransactionScheduleDto>
    {
        public TransactionScheduleDto Data;

        public CreateTransactionScheduleRequest(TransactionScheduleDto transaction)
        {
            Data = transaction;
        }
    }

   
    public class CreateTransactionScheduleRequestValidator : AbstractValidator<CreateTransactionScheduleRequest>
    {
        public CreateTransactionScheduleRequestValidator()
        {
            RuleFor(x => x.Data.Description).NotEmpty();
            RuleFor(x => x.Data.Amount).NotEmpty();
            RuleFor(x => x.Data.BudgetCategoryId).NotEmpty();
        }
    }
}