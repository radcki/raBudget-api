using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.CreateBudgetCategory
{
    public class CreateBudgetCategoryRequest : IRequest<CreateBudgetCategoryResponse>
    {
        public BudgetCategoryDto Data;

        public CreateBudgetCategoryRequest(BudgetCategoryDto budgetCategory)
        {
            Data = budgetCategory;
        }
    }

    public class CreateBudgetCategoryResponse : BaseResponse<BudgetCategoryDto>
    {
    }

    public class CreateBudgetCategoryRequestValidator : AbstractValidator<CreateBudgetCategoryRequest>
    {
        public CreateBudgetCategoryRequestValidator(IAuthenticationProvider authenticationProvider, IBudgetRepository budgetRepository)
        {
            RuleFor(x => x.Data.Name).NotEmpty();
            RuleFor(x => x.Data.Budget).NotEmpty();

            var task = budgetRepository.ListAvailableBudgets(authenticationProvider.User.UserId);
            task.Wait();
            var availableBudgets = task.Result;
            RuleFor(x => availableBudgets.Any(s => s.Id == x.Data.Budget.BudgetId))
               .NotEqual(false)
               .WithMessage("Specified budget does not exist");
        }
    }
}