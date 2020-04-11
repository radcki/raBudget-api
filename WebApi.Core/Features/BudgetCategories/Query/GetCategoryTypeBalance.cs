using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Exceptions;
using raBudget.Core.Features.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Features.BudgetCategories.Query
{
    public class GetCategoryTypeBalance
    {
        public class Query : IRequest<IEnumerable<BudgetCategoryBalance>>
        {
            public int BudgetId { get; set; }
            public eBudgetCategoryType BudgetCategoryType { get; set; }

            public Query(int budgetId, eBudgetCategoryType budgetCategoryType)
            {
                BudgetId = budgetId;
                BudgetCategoryType = budgetCategoryType;
            }
        }

        public class Handler : BaseBudgetHandler<Query, IEnumerable<BudgetCategoryBalance>>
        {
            public Handler
            (IBudgetRepository repository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
            {
            }

            /// <inheritdoc />
            public override async Task<IEnumerable<BudgetCategoryBalance>> Handle(Query query, CancellationToken cancellationToken)
            {
                var repositoryResult = await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
                var budget = repositoryResult.FirstOrDefault(x => x.Id == query.BudgetId);
                if (budget == null)
                {
                    throw new NotFoundException("Budget was not found");
                }

                switch (query.BudgetCategoryType)
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
}