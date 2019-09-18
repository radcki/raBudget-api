using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.TransactionHandlers.UpdateTransaction
{
    public class UpdateTransactionHandler : IRequestHandler<UpdateTransactionRequest, UpdateTransactionResponse>
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public UpdateTransactionHandler(ITransactionRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<UpdateTransactionResponse> Handle(UpdateTransactionRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}