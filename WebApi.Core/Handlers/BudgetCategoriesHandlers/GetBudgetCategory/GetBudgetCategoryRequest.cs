using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.ListBudgetCategories;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.GetBudgetCategory
{
    public class GetBudgetCategoryRequest : IRequest<GetBudgetCategoryResponse>
    {
        public int BudgetCategoryId;
        public int BudgetId;

        public GetBudgetCategoryRequest(int budgetCategoryId, int budgetId)
        {
            BudgetCategoryId = budgetCategoryId;
            BudgetId = budgetId;
        }
    }

    public class GetBudgetCategoryResponse : BaseResponse<BudgetCategoryDto>
    {
    }

    public class GetBudgetCategoryRequestValidator : AbstractValidator<GetBudgetCategoryRequest>
    {
        public GetBudgetCategoryRequestValidator(IBudgetRepository budgetRepository, IAuthenticationProvider authenticationProvider)
        {
            RuleFor(x => x.BudgetCategoryId).NotEmpty();

            var task = budgetRepository.ListAvailableBudgets(authenticationProvider.User.UserId);
            task.Wait();
            var availableBudgets = task.Result;
            RuleFor(x => availableBudgets.Any(s => s.Id == x.BudgetId))
               .NotEqual(false)
               .WithMessage("Specified budget does not exist");
        }
    }

}
