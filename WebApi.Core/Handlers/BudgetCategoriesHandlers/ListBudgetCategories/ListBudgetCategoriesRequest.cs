using System.Collections.Generic;
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
    public class ListBudgetCategoriesRequest : IRequest<IEnumerable<BudgetCategoryDto>>
    {
        public int BudgetId;

        public ListBudgetCategoriesRequest(int budgetId)
        {
            BudgetId = budgetId;
        }
    }
    
    public class ListBudgetCategoriesValidator : AbstractValidator<ListBudgetCategoriesRequest>
    {
        public ListBudgetCategoriesValidator()
        {
            RuleFor(x => x.BudgetId).NotEmpty();
        }
    }
}