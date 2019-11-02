using FluentValidation;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers.DeleteTransactionSchedule
{
    public class DeleteTransactionScheduleRequest : IRequest<TransactionScheduleDto>
    {
        public int TransactionScheduleId;

        public DeleteTransactionScheduleRequest(int transactionScheduleId)
        {
            TransactionScheduleId = transactionScheduleId;
        }
    }

    public class DeleteTransactionScheduleRequestValidator : AbstractValidator<DeleteTransactionScheduleRequest>
    {
        public DeleteTransactionScheduleRequestValidator()
        {
            RuleFor(x => x.TransactionScheduleId).NotEmpty();
        }
    }
    
}
