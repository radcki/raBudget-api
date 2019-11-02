using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetHandlers.GetUnassignedFunds;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.ListAvailableBudgets
{
    public class GetUnassignedFundsHandler : BaseBudgetHandler<GetUnassignedFundsRequest, double>
    {
        public GetUnassignedFundsHandler(IBudgetRepository repository, 
                                           IMapper mapper, 
                                           IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
        {
        }

        /// <inheritdoc />
        public override async Task<double> Handle(GetUnassignedFundsRequest request, CancellationToken cancellationToken)
        {
            var repositoryResult = await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
            var budget = repositoryResult.FirstOrDefault(x => x.Id == request.BudgetId);
            if (budget == null)
            {
                throw new NotFoundException("Budget was not found");
            }

            return budget.UnassignedFunds();
        }
    }
    
}