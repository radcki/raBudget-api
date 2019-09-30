using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.UserHandlers.SetDefaultBudget
{
    public class SetDefaultBudgetRequest : IRequest
    {
        public int BudgetId;

        public SetDefaultBudgetRequest(int budgetId)
        {
            BudgetId = budgetId;
        }
    }


    public class SetDefaultBudgetRequestValidator : AbstractValidator<SetDefaultBudgetRequest>
    {
        public SetDefaultBudgetRequestValidator(IBudgetRepository budgetRepository, IAuthenticationProvider authenticationProvider)
        {
            RuleFor(x => x.BudgetId).NotEmpty();

            /* Check if user has access to budget */
            var availableBudgets = budgetRepository.ListAvailableBudgets(authenticationProvider.User.UserId).Result;
            RuleFor(x => availableBudgets.Any(s => s.Id == x.BudgetId)).NotEqual(false)
                                                                       .WithMessage("Requested budget does not exist.");
        }
    }
    
}
