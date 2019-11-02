using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers
{
    public abstract class BaseTransactionScheduleHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly ITransactionScheduleRepository TransactionScheduleRepository;
        protected readonly IBudgetCategoryRepository BudgetCategoryRepository;
        protected readonly IMapper Mapper;
        protected readonly IAuthenticationProvider AuthenticationProvider;

        public BaseTransactionScheduleHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         ITransactionScheduleRepository transactionScheduleRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider)
        {
            BudgetCategoryRepository = budgetCategoryRepository;
            TransactionScheduleRepository = transactionScheduleRepository;
            Mapper = mapper;
            AuthenticationProvider = authenticationProvider;
        }


        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}