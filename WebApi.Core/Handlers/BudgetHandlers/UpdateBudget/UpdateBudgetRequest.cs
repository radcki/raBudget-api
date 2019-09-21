using System.Linq;
using System.Threading;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetHandlers.UpdateBudget
{
    public class UpdateBudgetRequest : IRequest
    {
        public BudgetDto Data;

        public UpdateBudgetRequest(BudgetDto budget)
        {
            Data = budget;
        }
    }

    public class UpdateBudgetRequestValidator : AbstractValidator<UpdateBudgetRequest>
    {
        public UpdateBudgetRequestValidator()
        {
            RuleFor(x => x.Data.BudgetId).NotEmpty();
            RuleFor(x => x.Data.Name).NotEmpty();
            RuleFor(x => x.Data.OwnedByUser).NotEmpty();
            RuleFor(x => x.Data.Currency).NotEmpty();
            RuleFor(x => x.Data.StartingDate).NotEmpty();
        }

    }
    
}
