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
    public class CreateAllocation
    {
        public class Command : IRequest<AllocationDto>
        {
            public AllocationDto Data;

            public Command(AllocationDto allocation)
            {
                Data = allocation;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Data.Description).NotEmpty();
                RuleFor(x => x.Data.TargetBudgetCategoryId).NotEmpty();
                RuleFor(x => x.Data.AllocationDate).NotEmpty();
            }
        }

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
                if (!await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.TargetBudgetCategoryId))
                {
                    throw new NotFoundException("Target budget category was not found.");
                }

                request.Data.CreatedByUser = AuthenticationProvider.User;

                var allocationEntity = Mapper.Map<Domain.Entities.Allocation>(request.Data);
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
}