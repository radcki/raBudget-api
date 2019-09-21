using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

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
