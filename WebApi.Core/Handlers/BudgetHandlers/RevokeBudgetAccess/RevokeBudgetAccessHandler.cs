using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetHandlers.RevokeBudgetAccess
{
    public class RevokeBudgetShareHandler : IRequestHandler<RevokeBudgetShareRequest, RevokeBudgetShareResponse>
    {
        private readonly IBudgetRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public RevokeBudgetShareHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<RevokeBudgetShareResponse> Handle(RevokeBudgetShareRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}