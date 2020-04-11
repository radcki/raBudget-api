using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Features.Budget
{
    public abstract class BaseBudgetHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly IBudgetRepository BudgetRepository;
        protected readonly IMapper Mapper;
        protected readonly IAuthenticationProvider AuthenticationProvider;

        protected BaseBudgetHandler
        (IBudgetRepository budgetRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider)
        {
            BudgetRepository = budgetRepository;
            Mapper = mapper;
            AuthenticationProvider = authenticationProvider;
        }


        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}