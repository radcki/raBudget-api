using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.TransactionHandlers.GetTransaction
{
    public class GetTransactionHandler : IRequestHandler<GetTransactionRequest, GetTransactionResponse>
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public GetTransactionHandler(ITransactionRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<GetTransactionResponse> Handle(GetTransactionRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}