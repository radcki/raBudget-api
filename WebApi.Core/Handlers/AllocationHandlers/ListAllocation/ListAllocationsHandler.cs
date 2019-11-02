using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Handlers.AllocationHandlers.ListAllocation
{
    public class ListAllocationsHandler : BaseAllocationHandler<ListAllocationsRequest, IEnumerable<AllocationDto>>
    {

        public ListAllocationsHandler(IAllocationRepository allocationRepository,
                                       IMapper mapper,
                                       IAuthenticationProvider authenticationProvider) : base(null, allocationRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<IEnumerable<AllocationDto>> Handle(ListAllocationsRequest request, CancellationToken cancellationToken)
        {
            if (request.Filters == null)
            {
                request.Filters = new AllocationFilterDto();
            }
            var allocations = await AllocationRepository.ListWithFilter(Mapper.Map<Budget>(request.Budget), Mapper.Map<AllocationFilterModel>(request.Filters));
            
            return Mapper.Map<IEnumerable<AllocationDto>>(allocations);
        }
    }
}