using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.ListBudgetCategories;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetHandlers.GetBudget
{
    public class GetBudgetRequest : IRequest<BudgetDto>
    {
        public int BudgetId;

        public GetBudgetRequest(int budgetId)
        {
            BudgetId = budgetId;
        }
    }

    public class GetBudgetRequestValidator : AbstractValidator<GetBudgetRequest>
    {
        public GetBudgetRequestValidator()
        {
            RuleFor(x => x.BudgetId).NotEmpty();
        }
    }
}
