using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.ListAvailableBudgets
{
    public class ListAvailableBudgetsHandler : BaseBudgetHandler<ListAvailableBudgetsRequest, IEnumerable<BudgetDto>>
    {
        public ListAvailableBudgetsHandler(IBudgetRepository repository, 
                                           IMapper mapper, 
                                           IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
        {
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<BudgetDto>> Handle(ListAvailableBudgetsRequest request, CancellationToken cancellationToken)
        {
            var repositoryResult = await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);

            return Mapper.Map<IEnumerable<BudgetDto>>(repositoryResult);
        }
    }
    
}