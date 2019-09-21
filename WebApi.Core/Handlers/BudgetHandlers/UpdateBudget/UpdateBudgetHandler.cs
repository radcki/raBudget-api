using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.UpdateBudget
{
    public class UpdateBudgetHandler : BaseBudgetHandler<UpdateBudgetRequest, Unit>
    {
        public UpdateBudgetHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
        {
        }

        public override async Task<Unit> Handle(UpdateBudgetRequest request, CancellationToken cancellationToken)
        {
            var availableBudgets = await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);

            if (availableBudgets.All(s => s.Id != request.Data.BudgetId))
            {
                throw new NotFoundException("Budget was not found");
            }
            
            var budgetEntity = availableBudgets.FirstOrDefault(x => x.Id == request.Data.BudgetId);

            budgetEntity.Name = request.Data.Name;
            budgetEntity.CurrencyCode = request.Data.Currency.CurrencyCode;
            budgetEntity.OwnedByUserId = request.Data.OwnedByUser.UserId;

            await BudgetRepository.UpdateAsync(budgetEntity);
            await BudgetRepository.SaveChangesAsync(cancellationToken);

            return new Unit();
        }
    }
}