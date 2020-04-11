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
                if (!await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.TargetBudgetCategoryId))
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

                return Mapper.Map<AllocationDto>(savedAllocation);
            }
        }
    }
}