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

namespace raBudget.Core.Features.Allocation.Command
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
        public class Notification : INotification
        {
            public int BudgetId { get; set; }
        }

        /// <summary>
        /// Delete allocation by its id. In case of success, deleted allocation data is returned
        /// </summary>
        public class Handler : BaseAllocationHandler<Command, AllocationDto>
        {
            private readonly IMediator _mediator;

            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             IAllocationRepository allocationRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider, 
             IMediator mediator) : base(budgetCategoryRepository, allocationRepository, mapper, authenticationProvider)
            {
                _mediator = mediator;
            }

            public override async Task<AllocationDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var allocationEntity = await AllocationRepository.GetByIdAsync(request.AllocationId);
                if (allocationEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(allocationEntity.TargetBudgetCategoryId))
                {
                    throw new NotFoundException("Target allocation was not found.");
                }

                await AllocationRepository.DeleteAsync(allocationEntity);
                await AllocationRepository.SaveChangesAsync(cancellationToken);

                _ = _mediator.Publish(new Notification()
                                      {
                                          BudgetId = allocationEntity.TargetBudgetCategory.BudgetId,
                                      }, cancellationToken);
                return Mapper.Map<AllocationDto>(allocationEntity);
            }
        }
    }
}