using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;

namespace raBudget.Core.Handlers.BudgetHandlers.RevokeBudgetAccess
{
    public class RevokeBudgetShareRequest : IRequest
    {
        public BudgetShareDto Data;

        public RevokeBudgetShareRequest(BudgetShareDto budgetShare)
        {
            Data = budgetShare;
        }
    }

  
    public class RevokeBudgetShareRequestValidator : AbstractValidator<RevokeBudgetShareRequest>
    {
        public RevokeBudgetShareRequestValidator()
        {
            RuleFor(x => x.Data.AllowedUser).NotEmpty();
            RuleFor(x => x.Data.Budget).NotEmpty();
        }
    }
    
}
