using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Handlers.BudgetHandlers.ListAvailableBudgets;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.DeleteBudgetCategory
{
    public class DeleteBudgetCategoryRequest : IRequest<DeleteBudgetCategoryResponse>
    {
        public int BudgetCategoryId;

        public DeleteBudgetCategoryRequest(int budgetCategoryId)
        {
            BudgetCategoryId = budgetCategoryId;
        }
    }

    public class DeleteBudgetCategoryResponse : BaseResponse
    {
    }

    public class DeleteBudgetCategoryRequestValidator : AbstractValidator<DeleteBudgetCategoryRequest>
    {
        public DeleteBudgetCategoryRequestValidator(IAuthenticationProvider authenticationProvider, IBudgetRepository budgetRepository)
        {
            RuleFor(x => x.BudgetCategoryId).NotEmpty();

            var task = budgetRepository.ListAvailableBudgets(authenticationProvider.User.UserId);
            task.Wait();
            var availableBudgets = task.Result;
            RuleFor(x => availableBudgets.SelectMany(s => s.BudgetCategories)
                                         .Any(s => s.Id == x.BudgetCategoryId))
               .Equal(true)
               .WithMessage("Specified budget category does not exist'");
        }
    }
}