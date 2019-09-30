using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Transaction;

namespace raBudget.Core.Handlers.TransactionHandlers.CreateTransaction
{
    public class CreateTransactionRequest : IRequest<TransactionDetailsDto>
    {
        public TransactionDto Data;

        public CreateTransactionRequest(TransactionDto transaction)
        {
            Data = transaction;
        }
    }

   
    public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
    {
        public CreateTransactionRequestValidator()
        {
            RuleFor(x => x.Data.Description).NotEmpty();
            RuleFor(x => x.Data.BudgetCategoryId).NotEmpty();
            RuleFor(x => x.Data.TransactionDate).NotEmpty();
        }
    }
}