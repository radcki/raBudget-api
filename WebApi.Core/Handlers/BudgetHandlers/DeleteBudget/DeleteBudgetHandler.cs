using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.DeleteBudget
{
    public class DeleteBudgetHandler : IRequestHandler<DeleteBudgetRequest>
    {
        private readonly IBudgetRepository _repository;
        private readonly IAuthenticationProvider _authenticationProvider;

        public DeleteBudgetHandler(IBudgetRepository repository, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<Unit> Handle(DeleteBudgetRequest request, CancellationToken cancellationToken)
        {
            var budgetEntity = await _repository.GetByIdAsync(request.BudgetId);
            if (_authenticationProvider.User.UserId != budgetEntity.OwnedByUserId)
            {
                throw new NotFoundException("Requested budget was not found in user's owned budgets'");
            }

            await _repository.DeleteAsync(budgetEntity);
            await _repository.SaveChangesAsync(cancellationToken);

            return new Unit();
        }

    }
}