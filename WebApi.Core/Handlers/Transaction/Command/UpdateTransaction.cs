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

namespace raBudget.Core.Handlers.Transaction.Command
{
    public class UpdateTransaction
    {
        public class Request : IRequest<Response>, IHaveCustomMapping
        {
            public int TransactionId { get; set; }
            public int BudgetCategoryId { get; set; }
            public TransactionScheduleDto TransactionSchedule { get; set; }
            public string Description { get; set; }
            public double Amount { get; set; }
            public DateTime TransactionDate { get; set; }

            public void CreateMappings(Profile configuration)
            {
                // dto -> entity
                configuration.CreateMap<Request, Domain.Entities.Transaction>()
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

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.BudgetCategoryId).NotEmpty();
                RuleFor(x => x.TransactionDate).NotEmpty();
            }
        }

        public class Handler : BaseTransactionHandler<Request, Response>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             ITransactionRepository transactionRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var transaction = await TransactionRepository.GetByIdAsync(request.TransactionId);
                var budgetCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.BudgetCategoryId);
                if (!await budgetCategoryAccessible)
                {
                    throw new NotFoundException("Target budget category was not found.");
                }

                transaction.Description = request.Description;
                transaction.Amount = request.Amount;
                transaction.TransactionDateTime = request.TransactionDate;
                transaction.BudgetCategoryId = request.BudgetCategoryId;
                transaction.TransactionScheduleId = request.TransactionSchedule?.TransactionScheduleId;

                await TransactionRepository.UpdateAsync(transaction);
                await TransactionRepository.SaveChangesAsync(cancellationToken);

                return new Response(){Data = Mapper.Map<TransactionDetailsDto>(transaction) };
            }
        }
    }
}