using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.ListAvailableBudgets
{
    public class ListAvailableBudgetsHandler : IRequestHandler<ListAvailableBudgetsRequest, ListAvailableBudgetsResponse>
    {
        private readonly IBudgetRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public ListAvailableBudgetsHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        /// <inheritdoc />
        public async Task<ListAvailableBudgetsResponse> Handle(ListAvailableBudgetsRequest request, CancellationToken cancellationToken)
        {
            
            var repositoryResult = await _repository.ListAvailableBudgets(_mapper.Map<Domain.Entities.User>(_authenticationProvider.User));

            return new ListAvailableBudgetsResponse()
                   {
                       ResponseType = repositoryResult.Any()
                                          ? eResponseType.Success
                                          : eResponseType.NoDataFound,

                       Data = _mapper.Map<IEnumerable<BudgetDto>>(repositoryResult)
                   }; ;
        }
    }
    
}