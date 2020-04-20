using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Dto.User;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Features.Allocation.Command
{
    public class UpdateAllocation
    {
        public class Command : IRequest<AllocationDto>
        {
            public int AllocationId { get; set; }
            public int TargetBudgetCategoryId { get; set; }
            public int? SourceBudgetCategoryId { get; set; }
            public string Description { get; set; }
            public double Amount { get; set; }
            public DateTime AllocationDate { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly IMediator _mediator;

            public Validator(IMediator mediator)
            {
                _mediator = mediator;
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.TargetBudgetCategoryId).NotEmpty();
                RuleFor(x => x.Amount).NotEmpty();
            }
        }

        public class Notification : INotification
        {
            public int BudgetId { get; set; }
            public AllocationDto Allocation { get; set; }
        }

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
                var allocation = await AllocationRepository.GetByIdAsync(request.AllocationId);
                if (allocation == null)
                {
                    throw new NotFoundException("Target allocation was not found.");
                }

                var originalTargetCategoryAccessible = await BudgetCategoryRepository.IsAccessibleToUser(allocation.TargetBudgetCategoryId);
                var targetCategoryAccessible = await BudgetCategoryRepository.IsAccessibleToUser(request.TargetBudgetCategoryId);
                if (!targetCategoryAccessible || !originalTargetCategoryAccessible)
                {
                    throw new NotFoundException("Target budget category was not found.");
                }

                if (request.SourceBudgetCategoryId != null)
                {
                    var sourceCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(request.SourceBudgetCategoryId.Value);
                    if (!await sourceCategoryAccessible)
                    {
                        throw new NotFoundException("Source budget category was not found.");
                    }
                }

                allocation.Description = request.Description;
                allocation.AllocationDateTime = request.AllocationDate;
                allocation.TargetBudgetCategoryId = request.TargetBudgetCategoryId;
                allocation.SourceBudgetCategoryId = request.SourceBudgetCategoryId;
                allocation.Amount = request.Amount;

                await AllocationRepository.UpdateAsync(allocation);
                await AllocationRepository.SaveChangesAsync(cancellationToken);
                var dto = Mapper.Map<AllocationDto>(allocation);
                _ = _mediator.Publish(new Notification()
                                      {
                                          BudgetId = allocation.TargetBudgetCategory.BudgetId,
                                          Allocation = dto
                                      }, cancellationToken);
                return dto;
            }
        }
    }
}