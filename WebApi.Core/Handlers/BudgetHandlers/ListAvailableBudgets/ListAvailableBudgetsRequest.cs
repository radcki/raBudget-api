using System.Collections.Generic;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;

namespace raBudget.Core.Handlers.BudgetHandlers.ListAvailableBudgets
{
    public class ListAvailableBudgetsRequest : IRequest<ListAvailableBudgetsResponse>
    {
    }

    public class ListAvailableBudgetsResponse : BaseResponse<IEnumerable<BudgetDto>>
    {
    }

    public class ListAvailableBudgetsRequestValidator : AbstractValidator<ListAvailableBudgetsRequest>
    {
        public ListAvailableBudgetsRequestValidator(IAuthenticationProvider auth)
        {
            RuleFor(x => auth.IsAuthenticated).NotEqual(false);
        }
    }
    
}
