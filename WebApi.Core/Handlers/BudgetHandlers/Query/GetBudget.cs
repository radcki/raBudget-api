using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Common;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetHandlers.Query
{
    public class GetBudget
    {
        public class Query : IRequest<Response>
        {
            public int BudgetId { get; set; }

        }

        public class Response : BaseResponse<BudgetDto>
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
            public Handler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
            {
            }

            /// <inheritdoc />
            public override async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                if (!await BudgetRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.BudgetId))
                {
                    throw new NotFoundException("Budget was not found");
                }

                var repositoryResult = await BudgetRepository.GetByIdAsync(request.BudgetId);

                var dto = Mapper.Map<BudgetDto>(repositoryResult);

                return new Response() {Data = dto};
            }
        }
    }
}