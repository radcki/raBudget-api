using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

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
            RuleFor(x => x.Data.BudgetCategory).NotEmpty();
        }
    }
}