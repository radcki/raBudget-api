using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Dto.User;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Features.TransactionSchedule.Command
{
    public class CreateTransactionSchedule
    {
        public class Command : IRequest<TransactionScheduleDto>
        {
            public string Description { get; set; }

            public double Amount { get; set; }

            public int BudgetCategoryId { get; set; }

            public eFrequency Frequency { get; set; }

            public int PeriodStep { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime? EndDate { get; set; }

        }

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                // dto -> entity
                configuration.CreateMap<Command, Domain.Entities.TransactionSchedule>()
                             .ForMember(entity => entity.StartDate, opt => opt.MapFrom(dto => dto.StartDate))
                             .ForMember(entity => entity.EndDate, opt => opt.MapFrom(dto => dto.EndDate))
                             .ForMember(entity => entity.BudgetCategory, opt => opt.Ignore())
                             .ForMember(entity => entity.CreatedByUser, opt => opt.Ignore())
                             .ForMember(entity => entity.CreatedByUserId, opt => opt.Ignore())
                             .ForMember(entity => entity.BudgetCategoryId, opt => opt.MapFrom(dto => dto.BudgetCategoryId));
            }
        }


        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.Amount).NotEmpty();
                RuleFor(x => x.BudgetCategoryId).NotEmpty();
            }
        }

        public class Handler : BaseTransactionScheduleHandler<Command, TransactionScheduleDto>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             ITransactionScheduleRepository transactionRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<TransactionScheduleDto> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!await BudgetCategoryRepository.IsAccessibleToUser(request.BudgetCategoryId))
                {
                    throw new NotFoundException("Target budget category was not found.");
                }

                var transactionScheduleEntity = Mapper.Map<Domain.Entities.TransactionSchedule>(request);
                transactionScheduleEntity.CreatedByUserId = AuthenticationProvider.User.UserId;

                var savedTransactionSchedule = await TransactionScheduleRepository.AddAsync(transactionScheduleEntity);

                var addedRows = await TransactionScheduleRepository.SaveChangesAsync(cancellationToken);
                if (addedRows.IsNullOrDefault())
                {
                    throw new SaveFailureException(nameof(transactionScheduleEntity), transactionScheduleEntity);
                }

                return Mapper.Map<TransactionScheduleDto>(savedTransactionSchedule);
            }
        }
    }
}