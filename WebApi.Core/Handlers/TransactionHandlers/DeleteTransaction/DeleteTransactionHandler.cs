using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.TransactionHandlers.DeleteTransaction
{
    public class DeleteTransactionHandler : IRequestHandler<DeleteTransactionRequest, DeleteTransactionResponse>
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public DeleteTransactionHandler(ITransactionRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<DeleteTransactionResponse> Handle(DeleteTransactionRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}