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
            if (allocation == null)
            {
                throw new NotFoundException("Target allocation was not found.");
            }

            var originalTargetCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, allocation.TargetBudgetCategoryId);
            var targetCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.TargetBudgetCategoryId);
            if (!await targetCategoryAccessible || !await originalTargetCategoryAccessible)
            {
                throw new NotFoundException("Target budget category was not found.");
            }

            if (request.Data.SourceBudgetCategoryId != null)
            {
                var sourceCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.SourceBudgetCategoryId.Value);
                if (!await sourceCategoryAccessible)
                {
                    throw new NotFoundException("Source budget category was not found.");
                }
            }

            allocation.Description = request.Data.Description;
            allocation.AllocationDateTime = request.Data.AllocationDate;
            allocation.TargetBudgetCategoryId = request.Data.TargetBudgetCategoryId;
            allocation.SourceBudgetCategoryId = request.Data.SourceBudgetCategoryId;
            allocation.Amount = request.Data.Amount;

            await AllocationRepository.UpdateAsync(allocation);
            await AllocationRepository.SaveChangesAsync(cancellationToken);

            return Mapper.Map<AllocationDto>(allocation);
        }
    }
}