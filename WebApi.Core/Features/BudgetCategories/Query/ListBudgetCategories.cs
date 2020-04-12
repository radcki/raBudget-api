using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Features.BudgetCategories.Query
{
    public class ListBudgetCategories
    {
        public class Query : IRequest<IEnumerable<BudgetCategoryDto>>
        {
            public int BudgetId;

            public Query(int budgetId)
            {
                BudgetId = budgetId;
            }
        }

        public class ListBudgetCategoriesValidator : AbstractValidator<Query>
        {
            public ListBudgetCategoriesValidator()
            {
                RuleFor(x => x.BudgetId).NotEmpty();
            }
        }

        public class Handler : BaseBudgetCategoryHandler<Query, IEnumerable<BudgetCategoryDto>>
        {
            private protected readonly IBudgetRepository _budgetRepository;

            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             IBudgetRepository budgetRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
            {
                _budgetRepository = budgetRepository;
            }

            public override async Task<IEnumerable<BudgetCategoryDto>> Handle(Query query, CancellationToken cancellationToken)
            {
                var availableBudgets = await _budgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
                if (availableBudgets.All(x => x.Id == query.BudgetId))
                {
                    throw new NotFoundException("Target budget was not found");
                }

                var categories = await BudgetCategoryRepository.ListWithFilter(new Domain.Entities.Budget(query.BudgetId), new BudgetCategoryFilterModel()
                                                                                                                           {
                                                                                                                               OrderBy = x=>x.Order,
                                                                                                                               DataOrder = eDataOrder.Ascending
                                                                                                                           });
                return Mapper.Map<IEnumerable<BudgetCategoryDto>>(categories);
            }
        }
    }
}