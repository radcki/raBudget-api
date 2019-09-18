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
    public class CreateTransactionRequest : IRequest<CreateTransactionResponse>
    {
        public TransactionDto Data;

        public CreateTransactionRequest(TransactionDto transaction)
        {
            Data = transaction;
        }
    }

    public class CreateTransactionResponse : BaseResponse<TransactionDto>
    {
    }

    public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
    {
        public CreateTransactionRequestValidator(IBudgetRepository budgetRepository, IAuthenticationProvider authenticationProvider)
        {
            RuleFor(x => x.Data.Description).NotEmpty();
            RuleFor(x => x.Data.BudgetCategory).NotEmpty();

            var accessibleCategories = budgetRepository.ListAvailableBudgets(authenticationProvider.User.UserId)
                                                       .Result
                                                       .SelectMany(x => x.BudgetCategories);

            RuleFor(x => accessibleCategories.Any(s => s.Id == x.Data.BudgetCategory.CategoryId))
               .NotEqual(false)
               .WithMessage("Specified budget category does not exist");
        }
    }
}