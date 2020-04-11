using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Common;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Features.Budget.Query
{
    public class ListAvailableBudgets
    {
        public class Query : IRequest<Response>
        {
        }

        public class Response : CollectionResponse<BudgetDto>
        {
        }


        public class Handler : BaseBudgetHandler<Query, Response>
        {
            public Handler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
            {
            }

            /// <inheritdoc />
            public override async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var repositoryResult = await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);

                var data = Mapper.Map<IEnumerable<BudgetDto>>(repositoryResult);

                return new Response() {Data = data};
            }
        }
    }
}