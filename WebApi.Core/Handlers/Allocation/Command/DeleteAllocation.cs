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

namespace raBudget.Core.Handlers.Allocation.Command
{
    public class DeleteAllocation
    {
        public class Command : IRequest<AllocationDto>
        {
            public int AllocationId;

            public Command(int allocationId)
            {
                AllocationId = allocationId;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.AllocationId).NotEmpty();
            }
        }

        /// <summary>
        /// Delete allocation by its id. In case of success, deleted allocation data is returned
        /// </summary>
        public class Handler : BaseAllocationHandler<Command, AllocationDto>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             IAllocationRepository allocationRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, allocationRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<AllocationDto> Handle(Command request, CancellationToken cancellationToken)
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
}