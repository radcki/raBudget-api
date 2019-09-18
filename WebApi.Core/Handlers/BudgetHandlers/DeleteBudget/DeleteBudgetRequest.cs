using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetHandlers.DeleteBudget
{
    public class DeleteBudgetRequest : IRequest<DeleteBudgetResponse>
    {
        public int BudgetId;

        public DeleteBudgetRequest(int budgetId)
        {
            BudgetId = budgetId;
        }
    }

    public class DeleteBudgetResponse : BaseResponse
    {
    }

    public class DeleteBudgetRequestValidator : AbstractValidator<DeleteBudgetRequest>
    {
        public DeleteBudgetRequestValidator(IBudgetRepository budgetRepository, IAuthenticationProvider authenticationProvider)
        {
            RuleFor(x => x.BudgetId).NotEmpty();

            var task = budgetRepository.ListAvailableBudgets(authenticationProvider.User.UserId);
            task.Wait();
            var availableBudgets = task.Result;
            RuleFor(x => availableBudgets.Any(s => s.Id == x.BudgetId)).NotEqual(false);
        }
    }
    
}
