using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Features.Allocation.Query
{
    public class ListAllocations
    {
        public class Query : IRequest<IEnumerable<AllocationDto>>
        {
            public BudgetDto Budget { get; set; }
            public AllocationFilterDto Filters { get; set; }

            public Query(BudgetDto budget)
            {
                Budget = budget;
            }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.Budget).NotEmpty();
                RuleFor(x => x.Budget.BudgetId).NotEmpty();
            }
        }

        public class Handler : BaseAllocationHandler<Query, IEnumerable<AllocationDto>>
        {
            public Handler
            (IAllocationRepository allocationRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(null, allocationRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<IEnumerable<AllocationDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (request.Filters == null)
                {
                    request.Filters = new AllocationFilterDto();
                }

                var allocations = await AllocationRepository.ListWithFilter(Mapper.Map<Domain.Entities.Budget>(request.Budget), Mapper.Map<AllocationFilterModel>(request.Filters));

                return Mapper.Map<IEnumerable<AllocationDto>>(allocations);
            }
        }
    }
}