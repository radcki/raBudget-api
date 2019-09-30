using System;
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
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.ListAvailableBudgets
{
    public class GetCategoryTypeBalanceHandler : BaseBudgetHandler<GetCategoryTypeBalanceRequest, IEnumerable<BudgetCategoryBalance>>
    {
        public GetCategoryTypeBalanceHandler(IBudgetRepository repository, 
                                           IMapper mapper, 
                                           IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
        {
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<BudgetCategoryBalance>> Handle(GetCategoryTypeBalanceRequest request, CancellationToken cancellationToken)
        {
            var repositoryResult = await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
            var budget = repositoryResult.FirstOrDefault(x => x.Id == request.BudgetId);
            if (budget == null)
            {
                throw new NotFoundException("Budget was not found");
            }

            switch (request.BudgetCategoryType)
            {
                case eBudgetCategoryType.Spending:
                    return budget.SpendingCategoriesBalance;
                case eBudgetCategoryType.Income:
                    return budget.IncomeCategoriesBalance;
                case eBudgetCategoryType.Saving:
                    return budget.SavingCategoriesBalance;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
}