using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Transaction;

namespace raBudget.Core.Handlers.TransactionHandlers.GetTransaction
{
    public class GetTransactionRequest : IRequest<TransactionDetailsDto>
    {
        public int TransactionId;

        public GetTransactionRequest(int transactionId)
        {
            TransactionId = transactionId;
        }
    }

    public class GetTransactionRequestValidator : AbstractValidator<GetTransactionRequest>
    {
        public GetTransactionRequestValidator()
        {
            RuleFor(x => x.TransactionId).NotEmpty();
        }
    }
    
}
