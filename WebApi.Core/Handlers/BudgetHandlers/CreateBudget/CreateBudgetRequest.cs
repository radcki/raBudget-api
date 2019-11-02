using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;

namespace raBudget.Core.Handlers.BudgetHandlers.CreateBudget
{
    public class CreateBudgetRequest : IRequest<BudgetDto>
    {
        public BudgetDto Data;

        public CreateBudgetRequest(BudgetDto budget)
        {
            Data = budget;
        }
    }

    public class CreateBudgetRequestValidator : AbstractValidator<CreateBudgetRequest>
    {
        public CreateBudgetRequestValidator()
        {
            RuleFor(x => x.Data.Name).NotEmpty();
            RuleFor(x => x.Data.Currency).NotEmpty();
            RuleFor(x => x.Data.OwnedByUser).NotEmpty();
            RuleFor(x => x.Data.StartingDate).NotEmpty();
        }
    }
    
}
