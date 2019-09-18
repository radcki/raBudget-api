using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;

namespace raBudget.Core.Handlers.TransactionHandlers.UpdateTransaction
{
    public class UpdateTransactionRequest : IRequest<UpdateTransactionResponse>
    {
        public TransactionDto Data;

        public UpdateTransactionRequest(TransactionDto transaction)
        {
            Data = transaction;
        }
    }

    public class UpdateTransactionResponse : BaseResponse<TransactionDto>
    {
    }

    public class UpdateTransactionRequestValidator : AbstractValidator<UpdateTransactionRequest>
    {
        public UpdateTransactionRequestValidator()
        {
            RuleFor(x => x.Data.Description).NotEmpty();
            RuleFor(x => x.Data.BudgetCategory).NotEmpty();
        }
    }
    
}
