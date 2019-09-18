using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;

namespace raBudget.Core.Handlers.TransactionHandlers.DeleteTransaction
{
    public class DeleteTransactionRequest : IRequest<DeleteTransactionResponse>
    {
        public int TransactionId;

        public DeleteTransactionRequest(int transactionId)
        {
            TransactionId = transactionId;
        }
    }

    public class DeleteTransactionResponse : BaseResponse<TransactionDto>
    {
    }

    public class DeleteTransactionRequestValidator : AbstractValidator<DeleteTransactionRequest>
    {
        public DeleteTransactionRequestValidator()
        {
            RuleFor(x => x.TransactionId).NotEmpty();
        }
    }
    
}
