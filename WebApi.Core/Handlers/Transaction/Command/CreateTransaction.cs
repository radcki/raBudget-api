using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.Transaction.Command
{
    public class CreateTransaction
    {
        public class Request : IRequest<Response>, IHaveCustomMapping
        {
            public int BudgetCategoryId { get; set; }
            public int? TransactionScheduleId { get; set; }
            public string Description { get; set; }
            public double Amount { get; set; }
            public DateTime TransactionDate { get; set; }

            public void CreateMappings(Profile configuration)
            {
                // dto -> entity
                configuration.CreateMap<Request, Domain.Entities.Transaction>()
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
                if (!await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.BudgetCategoryId))
                {
                    throw new NotFoundException("Target budget category was not found.");
                }


                var transactionEntity = Mapper.Map<Domain.Entities.Transaction>(request);
                transactionEntity.CreatedByUserId = AuthenticationProvider.User.UserId;
                var savedTransaction = await TransactionRepository.AddAsync(transactionEntity);

                var addedRows = await TransactionRepository.SaveChangesAsync(cancellationToken);
                if (addedRows.IsNullOrDefault())
                {
                    throw new SaveFailureException(nameof(transactionEntity), transactionEntity);
                }

                return new Response { Data = Mapper.Map<TransactionDetailsDto>(savedTransaction) };
            }
        }
    }
}