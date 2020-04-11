using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Features.Transaction.Command
{
    public class DeleteTransaction
    {
        public class Request : IRequest<Response>
        {
            public int TransactionId { get; set; }
            
        }

        public class Response
        {
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.TransactionId).NotEmpty();
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
                var transactionEntity = await TransactionRepository.GetByIdAsync(request.TransactionId);
                if (transactionEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, transactionEntity.BudgetCategoryId))
                {
                    throw new NotFoundException("Target transaction was not found.");
                }

                await TransactionRepository.DeleteAsync(transactionEntity);
                await TransactionRepository.SaveChangesAsync(cancellationToken);

                return new Response();
            }
        }
    }
}