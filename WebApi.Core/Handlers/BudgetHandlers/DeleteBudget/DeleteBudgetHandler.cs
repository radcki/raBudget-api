using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.DeleteBudget
{
    public class DeleteBudgetHandler : IRequestHandler<DeleteBudgetRequest, DeleteBudgetResponse>
    {
        private readonly IBudgetRepository _repository;
        private readonly IAuthenticationProvider _authenticationProvider;

        public DeleteBudgetHandler(IBudgetRepository repository, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<DeleteBudgetResponse> Handle(DeleteBudgetRequest request, CancellationToken cancellationToken)
        {
            var budgetEntity = await _repository.GetByIdAsync(request.BudgetId);
            if (_authenticationProvider.User.UserId != budgetEntity.OwnedByUserId)
            {
                return new DeleteBudgetResponse() {ResponseType = eResponseType.Unauthorized};
            }

            await _repository.DeleteAsync(budgetEntity);
            await _repository.SaveChangesAsync(cancellationToken);
            return new DeleteBudgetResponse()
                   {
                       ResponseType = eResponseType.Success
                   };
        }
    }
}