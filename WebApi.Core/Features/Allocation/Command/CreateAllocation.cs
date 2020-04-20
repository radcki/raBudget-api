using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.User;
using raBudget.Core.Exceptions;
using raBudget.Core.Features.Transaction.Command;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Features.Allocation.Command
{
    public class CreateAllocation
    {
        public class Command : IRequest<AllocationDto>
        {
            public int TargetBudgetCategoryId { get; set; }
            public int? SourceBudgetCategoryId { get; set; }
            public string Description { get; set; }
            public double Amount { get; set; }
            public int BudgetId { get; set; }
            public DateTime AllocationDate { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.TargetBudgetCategoryId).NotEmpty();
                RuleFor(x => x.AllocationDate).NotEmpty();
            }
        }

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                // dto -> entity
                configuration.CreateMap<Command, Domain.Entities.Allocation>()
                             .ForMember(entity => entity.AllocationDateTime, opt => opt.MapFrom(dto => dto.AllocationDate))
                             .ForMember(entity => entity.TargetBudgetCategory, opt => opt.Ignore())
                             .ForMember(entity => entity.SourceBudgetCategory, opt => opt.Ignore())
                             .ForMember(entity => entity.CreatedByUser, opt => opt.Ignore())
                             .ForMember(entity => entity.TargetBudgetCategoryId, opt => opt.MapFrom(dto => dto.TargetBudgetCategoryId));
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
                var category = await BudgetCategoryRepository.GetByIdAsync(request.TargetBudgetCategoryId);
                if (category == null)
                {
                    throw new NotFoundException("Target budget category was not found.");
                }


                var allocationEntity = Mapper.Map<Domain.Entities.Allocation>(request);
                allocationEntity.CreatedByUserId = AuthenticationProvider.User.UserId;
                allocationEntity.CreationDateTime = DateTime.Now;

                var savedAllocation = await AllocationRepository.AddAsync(allocationEntity);

                var addedRows = await AllocationRepository.SaveChangesAsync(cancellationToken);
                if (addedRows.IsNullOrDefault())
                {
                    throw new SaveFailureException(nameof(allocationEntity), allocationEntity);
                }

                var dto = Mapper.Map<AllocationDto>(savedAllocation);
                _ = _mediator.Publish(new Notification()
                                      {
                                          BudgetId = category.BudgetId,
                                          Allocation = dto
                                      }, cancellationToken);
                return dto;
            }
        }
    }
}