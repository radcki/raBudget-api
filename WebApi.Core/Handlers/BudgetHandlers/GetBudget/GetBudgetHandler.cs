using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.ExtensionMethods;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.GetBudget
{
    public class GetBudgetHandler : IRequestHandler<GetBudgetRequest, GetBudgetResponse>
    {
        private readonly IBudgetRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public GetBudgetHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        /// <inheritdoc />
        public async Task<GetBudgetResponse> Handle(GetBudgetRequest request, CancellationToken cancellationToken)
        {
            var repositoryResult = await _repository.GetByIdAsync(request.Id);
            if (repositoryResult.IsNullOrDefault()
                || (repositoryResult.OwnedByUserId != _authenticationProvider.User.UserId
                    && repositoryResult.BudgetShares.All(x=>x.SharedWithUserId != _authenticationProvider.User.UserId)))
            {
                return new GetBudgetResponse() {ResponseType = eResponseType.NoDataFound };
            }

            var dto = _mapper.Map<BudgetDetailsDto>(repositoryResult);

            return new GetBudgetResponse()
                   {
                       ResponseType = eResponseType.Success,
                       Data = dto
                   };
        }
    }
    
}