using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Features.BudgetCategories.Query
{
    public class GetBudgetCategory
    {
        public class Query : IRequest<BudgetCategoryDto>
        {
            public int BudgetCategoryId;
            public int BudgetId;

            public Query(int budgetCategoryId, int budgetId)
            {
                BudgetCategoryId = budgetCategoryId;
                BudgetId = budgetId;
            }
        }


        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.BudgetCategoryId).NotEmpty();
            }
        }

        public class Handler : BaseBudgetCategoryHandler<Query, BudgetCategoryDto>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<BudgetCategoryDto> Handle(Query query, CancellationToken cancellationToken)
            {
                var isAccessible = await BudgetCategoryRepository.IsAccessibleToUser(query.BudgetCategoryId);
                if (!isAccessible)
                {
                    throw new NotFoundException("Specified budget does not exist");
                }

                var budgetCategory = await BudgetCategoryRepository.GetByIdAsync(query.BudgetCategoryId);

                return Mapper.Map<BudgetCategoryDto>(budgetCategory);
            }
        }
    }
}