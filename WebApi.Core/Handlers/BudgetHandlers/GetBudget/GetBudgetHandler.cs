using System.Data;
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

namespace raBudget.Core.Handlers.BudgetHandlers.GetBudget
{
    public class GetBudgetHandler : BaseBudgetHandler<GetBudgetRequest, BudgetDto>
    {
        public GetBudgetHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository,mapper, authenticationProvider)
        {
        }

        /// <inheritdoc />
        public override async Task<BudgetDto> Handle(GetBudgetRequest request, CancellationToken cancellationToken)
        {
            var availableBudgets =  await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
            if (availableBudgets.All(s => s.Id != request.BudgetId))
            {
                throw new NotFoundException("Budget was not found");
            }

            var repositoryResult = await BudgetRepository.GetByIdAsync(request.BudgetId);
            
            var dto = Mapper.Map<BudgetDto>(repositoryResult);

            return dto;
        }
    }
    
}