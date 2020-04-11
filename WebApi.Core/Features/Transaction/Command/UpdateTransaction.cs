using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Features.Transaction.Command
{
    public class UpdateTransaction
    {
        public class Command : IRequest<Response>
        {
            public int TransactionId { get; set; }
            public int BudgetCategoryId { get; set; }
            public int? TransactionScheduleId { get; set; }
            public string Description { get; set; }
            public double Amount { get; set; }
            public DateTime TransactionDate { get; set; }
        }

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                // dto -> entity
                configuration.CreateMap<Command, Domain.Entities.Transaction>()
                             .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.TransactionId))
                             .ForMember(entity => entity.TransactionDateTime, opt => opt.MapFrom(dto => dto.TransactionDate))
                             .ForMember(entity => entity.BudgetCategory, opt => opt.Ignore())
                             .ForMember(entity => entity.CreatedByUser, opt => opt.Ignore())
                             .ForMember(entity => entity.BudgetCategoryId, opt => opt.MapFrom(dto => dto.BudgetCategoryId));
            }
        }

        public class Response
        {
            public TransactionDetailsDto Data { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.BudgetCategoryId).NotEmpty();
                RuleFor(x => x.TransactionDate).NotEmpty();
            }
        }

        public class Handler : BaseTransactionHandler<Command, Response>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             ITransactionRepository transactionRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                var transaction = await TransactionRepository.GetByIdAsync(command.TransactionId);
                var budgetCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, command.BudgetCategoryId);
                if (!await budgetCategoryAccessible)
                {
                    throw new NotFoundException("Target budget category was not found.");
                }

                transaction.Description = command.Description;
                transaction.Amount = command.Amount;
                transaction.TransactionDateTime = command.TransactionDate;
                transaction.BudgetCategoryId = command.BudgetCategoryId;
                transaction.TransactionScheduleId = command.TransactionScheduleId;

                await TransactionRepository.UpdateAsync(transaction);
                await TransactionRepository.SaveChangesAsync(cancellationToken);

                return new Response() {Data = Mapper.Map<TransactionDetailsDto>(transaction)};
            }
        }
    }
}