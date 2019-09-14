using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget.Response;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces;

namespace raBudget.Core.Dto.Budget.Request
{
    public class ListAvailableBudgetsRequest : IRequest<ListAvailableBudgetsResponse>
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
