using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.AllocationHandlers.UpdateAllocation
{
    public class UpdateAllocationHandler : BaseAllocationHandler<UpdateAllocationRequest, AllocationDto>
    {
        public UpdateAllocationHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IAllocationRepository allocationRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, allocationRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<AllocationDto> Handle(UpdateAllocationRequest request, CancellationToken cancellationToken)
        {
            var allocation = await AllocationRepository.GetByIdAsync(request.Data.AllocationId);
            var sourceCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, allocation.Id);
            var targetCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.AllocationId);
            if (!await sourceCategoryAccessible)
            {
                throw new NotFoundException("Source budget category was not found.");
            }

            if (!await targetCategoryAccessible)
            {
                throw new NotFoundException("Target budget category was not found.");
            }

            allocation.Description = request.Data.Description;
            allocation.AllocationDateTime = request.Data.AllocationDate;
            allocation.TargetBudgetCategoryId = request.Data.TargetBudgetCategoryId;

            await AllocationRepository.UpdateAsync(allocation);
            await AllocationRepository.SaveChangesAsync(cancellationToken);

            return Mapper.Map<AllocationDto>(allocation);
        }
    }
}