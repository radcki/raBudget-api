using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.TransactionHandlers.Query
{
    public class GetTransaction
    {
        public class Query : IRequest<Response>
        {
            public int TransactionId { get; set; }
        }

        public class Response
        {
            public TransactionDetailsDto Data;
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.TransactionId).NotEmpty();
            }
        }

        public class Handler : BaseTransactionHandler<Query, Response>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             ITransactionRepository transactionRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var transactionEntity = await TransactionRepository.GetByIdAsync(request.TransactionId);
                if (transactionEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, transactionEntity.Id))
                {
                    throw new NotFoundException("Target transaction was not found.");
                }

                return new Response { Data = Mapper.Map<TransactionDetailsDto>(transactionEntity) };
            }
        }
    }
}