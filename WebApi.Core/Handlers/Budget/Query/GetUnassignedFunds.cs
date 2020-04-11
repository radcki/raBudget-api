using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Common;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.Budget.Query
{
    public class GetUnassignedFunds
    {
        public class Query : IRequest<Response>
        {
            public int BudgetId;
        }

        public class Response : BaseResponse<double>
        {
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.BudgetId).NotEmpty();
            }
        }

        public class Handler : BaseBudgetHandler<Query, Response>
        {
            public Handler(IBudgetRepository repository,  IAuthenticationProvider authenticationProvider) : base(repository, null, authenticationProvider)
            {
            }

            /// <inheritdoc />
            public override async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var repositoryResult = await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
                var budget = repositoryResult.FirstOrDefault(x => x.Id == request.BudgetId);
                if (budget == null)
                {
                    throw new NotFoundException("Budget was not found");
                }

                return new Response(){Data = budget.UnassignedFunds() };
            }
        }
    }
}