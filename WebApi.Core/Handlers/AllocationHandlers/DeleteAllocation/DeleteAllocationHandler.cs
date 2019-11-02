using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Handlers.AllocationHandlers.CreateAllocation;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.AllocationHandlers.DeleteAllocation
{
    /// <summary>
    /// Delete allocation by its id. In case of success, deleted allocation data is returned
    /// </summary>
    public class DeleteAllocationHandler : BaseAllocationHandler<DeleteAllocationRequest, AllocationDto>
    {
        public DeleteAllocationHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IAllocationRepository allocationRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, allocationRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<AllocationDto> Handle(DeleteAllocationRequest request, CancellationToken cancellationToken)
        {
            var allocationEntity = await AllocationRepository.GetByIdAsync(request.AllocationId);
            if (allocationEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, allocationEntity.TargetBudgetCategoryId))
            {
                throw new NotFoundException("Target allocation was not found.");
            }

            await AllocationRepository.DeleteAsync(allocationEntity);
            await AllocationRepository.SaveChangesAsync(cancellationToken);

            return Mapper.Map<AllocationDto>(allocationEntity);
        }
    }
}