using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.UpdateBudgetCategory
{
    public class UpdateBudgetCategoryRequest : IRequest<UpdateBudgetCategoryResponse>
    {
        public BudgetCategoryDto Data;

        public UpdateBudgetCategoryRequest(BudgetCategoryDto budgetCategory)
        {
            Data = budgetCategory;
        }
    }

    public class UpdateBudgetCategoryResponse : BaseResponse<BudgetCategoryDto>
    {
    }

    public class UpdateBudgetCategoryRequestValidator : AbstractValidator<UpdateBudgetCategoryRequest>
    {
        public UpdateBudgetCategoryRequestValidator(IBudgetRepository budgetRepository, IAuthenticationProvider authenticationProvider)
        {
            RuleFor(x => x.Data.Name).NotEmpty();

            var task = budgetRepository.ListAvailableBudgets(authenticationProvider.User.UserId);
            task.Wait();
            var availableBudgets = task.Result;
            RuleFor(x => availableBudgets.SelectMany(s=>s.BudgetCategories)
                                         .Any(s => s.Id == x.Data.CategoryId))
               .NotEqual(false)
               .WithMessage("Specified budget category does not exist");
        }
    }
    
}
