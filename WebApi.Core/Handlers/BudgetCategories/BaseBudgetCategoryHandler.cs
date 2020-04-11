using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetCategories
{
    public abstract class BaseBudgetCategoryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly IBudgetCategoryRepository BudgetCategoryRepository;
        protected readonly IMapper Mapper;
        protected readonly IAuthenticationProvider AuthenticationProvider;

        protected BaseBudgetCategoryHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider)
        {
            BudgetCategoryRepository = budgetCategoryRepository;
            Mapper = mapper;
            AuthenticationProvider = authenticationProvider;
        }


        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}