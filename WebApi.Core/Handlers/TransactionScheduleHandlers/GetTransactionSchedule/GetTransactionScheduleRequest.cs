using FluentValidation;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers.GetTransactionSchedule
{
    public class GetTransactionScheduleRequest : IRequest<TransactionScheduleDetailsDto>
    {
        public int TransactionScheduleId;

        public GetTransactionScheduleRequest(int transactionId)
        {
            TransactionScheduleId = transactionId;
        }
    }

    public class GetTransactionScheduleRequestValidator : AbstractValidator<GetTransactionScheduleRequest>
    {
        public GetTransactionScheduleRequestValidator()
        {
            RuleFor(x => x.TransactionScheduleId).NotEmpty();
        }
    }
    
}
