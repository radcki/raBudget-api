using FluentValidation;
using MediatR;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.DeleteBudgetCategory
{
    public class DeleteBudgetCategoryRequest : IRequest
    {
        public int BudgetCategoryId;

        public DeleteBudgetCategoryRequest(int budgetCategoryId)
        {
            BudgetCategoryId = budgetCategoryId;
        }
    }
    
    public class DeleteBudgetCategoryRequestValidator : AbstractValidator<DeleteBudgetCategoryRequest>
    {
        public DeleteBudgetCategoryRequestValidator()
        {
            RuleFor(x => x.BudgetCategoryId).NotEmpty();
        }
    }
}