using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Handlers.AllocationHandlers.DeleteAllocation;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.AllocationHandlers.GetAllocation
{
    public class GetAllocationHandler : BaseAllocationHandler<GetAllocationRequest, AllocationDto>
    {
        public GetAllocationHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IAllocationRepository allocationRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, allocationRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<AllocationDto> Handle(GetAllocationRequest request, CancellationToken cancellationToken)
        {
            var allocationEntity = await AllocationRepository.GetByIdAsync(request.AllocationId);
            if (allocationEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, allocationEntity.Id))
            {
                throw new NotFoundException("Target allocation was not found.");
            }

            return Mapper.Map<AllocationDto>(allocationEntity);
        }
    }
}