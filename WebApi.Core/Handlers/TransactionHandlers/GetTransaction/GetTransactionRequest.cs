using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;

namespace raBudget.Core.Handlers.TransactionHandlers.GetTransaction
{
    public class GetTransactionRequest : IRequest<GetTransactionResponse>
    {
        public int TransactionId;

        public GetTransactionRequest(int transactionId)
        {
            TransactionId = transactionId;
        }
    }

    public class GetTransactionResponse : BaseResponse<TransactionDto>
    {
    }

    public class GetTransactionRequestValidator : AbstractValidator<GetTransactionRequest>
    {
        public GetTransactionRequestValidator()
        {
            RuleFor(x => x.TransactionId).NotEmpty();
        }
    }
    
}
