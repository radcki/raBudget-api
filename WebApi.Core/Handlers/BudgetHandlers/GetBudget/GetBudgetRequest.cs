using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.ListBudgetCategories;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetHandlers.GetBudget
{
    public class GetBudgetRequest : IRequest<GetBudgetResponse>
    {
        public int BudgetId;

        public GetBudgetRequest(int budgetId)
        {
            BudgetId = budgetId;
        }
    }

    public class GetBudgetResponse : BaseResponse<BudgetDetailsDto>
    {
    }

    public class GetBudgetRequestValidator : AbstractValidator<GetBudgetRequest>
    {
        public GetBudgetRequestValidator(IBudgetRepository budgetRepository, IAuthenticationProvider authenticationProvider)
        {
            RuleFor(x => x.BudgetId).NotEmpty();

            var task = budgetRepository.ListAvailableBudgets(authenticationProvider.User.UserId);
            task.Wait();
            var availableBudgets = task.Result;
            RuleFor(x => availableBudgets.Any(s => s.Id == x.BudgetId)).NotEqual(false);
        }
    }
}
