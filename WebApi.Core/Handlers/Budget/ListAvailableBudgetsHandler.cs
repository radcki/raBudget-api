using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Budget.Request;
using raBudget.Core.Dto.Budget.Response;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.Budget
{
    public class ListAvailableBudgetsHandler : IRequestHandler<ListAvailableBudgetsRequest, ListAvailableBudgetsResponse>
    {
        private readonly IBudgetRepository<Domain.Entities.Budget> _repository;
        private readonly IMapper _mapper;
        public readonly IAuthenticationProvider _authenticationProvider;

        public ListAvailableBudgetsHandler(IBudgetRepository<Domain.Entities.Budget> repository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        /// <inheritdoc />
        public async Task<ListAvailableBudgetsResponse> Handle(ListAvailableBudgetsRequest request, CancellationToken cancellationToken)
        {
            
            var repositoryResult = await _repository.ListAvailableBudgets(_mapper.Map<User>(_authenticationProvider.User));

            return new ListAvailableBudgetsResponse()
                   {
                       ResponseType = repositoryResult.Any()
                                          ? eResponseType.Success
                                          : eResponseType.NotFound,

                       Data = _mapper.Map<IEnumerable<BudgetDto>>(repositoryResult)
                   }; ;
        }
    }
    
}