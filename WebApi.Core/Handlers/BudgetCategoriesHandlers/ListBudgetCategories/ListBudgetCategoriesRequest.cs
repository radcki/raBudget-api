using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.UpdateBudgetCategory;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.ListBudgetCategories
{
    public class ListBudgetCategoriesRequest : IRequest<ListBudgetCategoriesResponse>
    {
        public int BudgetId;

        public ListBudgetCategoriesRequest(int budgetId)
        {
            BudgetId = budgetId;
        }
    }

    public class ListBudgetCategoriesResponse : BaseResponse<BudgetCategoryDto>
    {
    }

    public class ListBudgetCategoriesValidator : AbstractValidator<ListBudgetCategoriesRequest>
    {
        public ListBudgetCategoriesValidator(IBudgetRepository budgetRepository, IAuthenticationProvider authenticationProvider)
        {
            var task = budgetRepository.ListAvailableBudgets(authenticationProvider.User.UserId);
            task.Wait();
            var availableBudgets = task.Result;
            RuleFor(x => availableBudgets.Any(s => s.Id == x.BudgetId))
               .NotEqual(false)
               .WithMessage("Specified budget does not exist");
        }
    }
}