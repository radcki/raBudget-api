using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;

namespace raBudget.Core.Handlers.BudgetHandlers.GetBudget
{
    public class GetBudgetRequest : IRequest<GetBudgetResponse>
    {
        public int Id;

        public GetBudgetRequest(int budgetId)
        {
            Id = budgetId;
        }
    }

    public class GetBudgetResponse : BaseResponse<BudgetDetailsDto>
    {
    }

    public class GetBudgetRequestValidator : AbstractValidator<GetBudgetRequest>
    {
        public GetBudgetRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    
}
