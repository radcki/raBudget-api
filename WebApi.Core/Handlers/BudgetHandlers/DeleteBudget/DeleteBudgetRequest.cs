using FluentValidation;
using MediatR;

namespace raBudget.Core.Handlers.BudgetHandlers.DeleteBudget
{
    public class DeleteBudgetRequest : IRequest
    {
        public int BudgetId;

        public DeleteBudgetRequest(int budgetId)
        {
            BudgetId = budgetId;
        }
    }

    public class DeleteBudgetRequestValidator : AbstractValidator<DeleteBudgetRequest>
    {
        public DeleteBudgetRequestValidator()
        {
            RuleFor(x => x.BudgetId).NotEmpty();
        }
    }
    
}
