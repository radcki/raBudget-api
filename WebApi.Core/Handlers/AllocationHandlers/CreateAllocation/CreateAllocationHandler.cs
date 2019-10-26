using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.AllocationHandlers.CreateAllocation
{
    public class CreateAllocationHandler : BaseAllocationHandler<CreateAllocationRequest, AllocationDto>
    {
        public CreateAllocationHandler(IBudgetCategoryRepository budgetCategoryRepository,
                                        IAllocationRepository allocationRepository, 
                                        IMapper mapper, 
                                        IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, allocationRepository, mapper, authenticationProvider)
        {

        }

        public override async Task<AllocationDto> Handle(CreateAllocationRequest request, CancellationToken cancellationToken)
        {
            if (! await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.TargetBudgetCategoryId))
            {
                throw new NotFoundException("Target budget category was not found.");
            }

            request.Data.CreatedByUser = AuthenticationProvider.User;

            var allocationEntity = Mapper.Map<Allocation>(request.Data);
            var savedAllocation = await AllocationRepository.AddAsync(allocationEntity);

            var addedRows = await AllocationRepository.SaveChangesAsync(cancellationToken);
            if (addedRows.IsNullOrDefault())
            {
                throw new SaveFailureException(nameof(allocationEntity), allocationEntity);
            }

            return Mapper.Map<AllocationDto>(savedAllocation);
        }
    }
}