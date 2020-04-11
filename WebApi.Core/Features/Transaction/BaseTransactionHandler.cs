using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Features.Transaction
{
    public abstract class BaseTransactionHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly ITransactionRepository TransactionRepository;
        protected readonly IBudgetCategoryRepository BudgetCategoryRepository;
        protected readonly IMapper Mapper;
        protected readonly IAuthenticationProvider AuthenticationProvider;

        protected BaseTransactionHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         ITransactionRepository transactionRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider)
        {
            BudgetCategoryRepository = budgetCategoryRepository;
            TransactionRepository = transactionRepository;
            Mapper = mapper;
            AuthenticationProvider = authenticationProvider;
        }


        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}