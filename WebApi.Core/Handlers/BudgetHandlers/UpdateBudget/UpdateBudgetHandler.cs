using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.UpdateBudget
{
    public class UpdateBudgetHandler : IRequestHandler<UpdateBudgetRequest, UpdateBudgetResponse>
    {
        private readonly IBudgetRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public UpdateBudgetHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<UpdateBudgetResponse> Handle(UpdateBudgetRequest request, CancellationToken cancellationToken)
        {
            var budgetEntity = await _repository.GetByIdAsync(request.Data.BudgetId);

            budgetEntity.Name = request.Data.Name;
            budgetEntity.CurrencyCode = request.Data.Currency.CurrencyCode;
            budgetEntity.OwnedByUserId = request.Data.OwnedByUser.UserId;

            await _repository.UpdateAsync(budgetEntity);
            await _repository.SaveChangesAsync(cancellationToken);
            return new UpdateBudgetResponse()
                   {
                       ResponseType = eResponseType.Success,
                       Data = _mapper.Map<BudgetDto>(budgetEntity)
                   };
        }
    }
}