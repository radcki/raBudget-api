using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.CreateBudgetCategory
{
    public class CreateBudgetCategoryRequest : IRequest<BudgetCategoryDto>
    {
        public BudgetCategoryDto Data;

        public CreateBudgetCategoryRequest(BudgetCategoryDto budgetCategory)
        {
            Data = budgetCategory;
        }
    }

    public class CreateBudgetCategoryRequestValidator : AbstractValidator<CreateBudgetCategoryRequest>
    {
        public CreateBudgetCategoryRequestValidator()
        {
            RuleFor(x => x.Data.Name).NotEmpty();
            RuleFor(x => x.Data.Budget).NotEmpty();
           
        }
    }
}