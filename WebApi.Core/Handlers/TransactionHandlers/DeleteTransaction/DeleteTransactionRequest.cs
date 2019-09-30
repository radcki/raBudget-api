using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Transaction;

namespace raBudget.Core.Handlers.TransactionHandlers.DeleteTransaction
{
    public class DeleteTransactionRequest : IRequest<TransactionDto>
    {
        public int TransactionId;

        public DeleteTransactionRequest(int transactionId)
        {
            TransactionId = transactionId;
        }
    }

    public class DeleteTransactionRequestValidator : AbstractValidator<DeleteTransactionRequest>
    {
        public DeleteTransactionRequestValidator()
        {
            RuleFor(x => x.TransactionId).NotEmpty();
        }
    }
    
}
