using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Features.Allocation.Query
{
    public class GetAllocation
    {
        public class Query : IRequest<AllocationDto>
        {
            public int AllocationId;

            public Query(int allocationId)
            {
                AllocationId = allocationId;
            }
        }

        public class GetAllocationRequestValidator : AbstractValidator<Query>
        {
            public GetAllocationRequestValidator()
            {
                RuleFor(x => x.AllocationId).NotEmpty();
            }
        }

        public class Handler : BaseAllocationHandler<Query, AllocationDto>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             IAllocationRepository allocationRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, allocationRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<AllocationDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var allocationEntity = await AllocationRepository.GetByIdAsync(request.AllocationId);
                if (allocationEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(allocationEntity.Id))
                {
                    throw new NotFoundException("Target allocation was not found.");
                }

                return Mapper.Map<AllocationDto>(allocationEntity);
            }
        }
    }
}