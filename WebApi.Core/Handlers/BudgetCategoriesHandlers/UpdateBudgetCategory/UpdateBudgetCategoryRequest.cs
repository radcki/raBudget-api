using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.UpdateBudgetCategory
{
    public class UpdateBudgetCategoryRequest : IRequest<BudgetCategoryDto>
    {
        public BudgetCategoryDto Data;

        public UpdateBudgetCategoryRequest(BudgetCategoryDto budgetCategory)
        {
            Data = budgetCategory;
        }
    }

    public class UpdateBudgetCategoryRequestValidator : AbstractValidator<UpdateBudgetCategoryRequest>
    {
        public UpdateBudgetCategoryRequestValidator()
        {
            RuleFor(x => x.Data.Name).NotEmpty();
            RuleFor(x => x.Data.CategoryId).NotEmpty();
        }
    }
    
}
