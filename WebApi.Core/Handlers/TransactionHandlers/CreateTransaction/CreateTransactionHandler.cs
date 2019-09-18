using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Exceptions;
using raBudget.Core.ExtensionMethods;
using raBudget.Core.Handlers.BudgetHandlers.CreateBudget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.TransactionHandlers.CreateTransaction
{
    public class CreateTransactionHandler : IRequestHandler<CreateTransactionRequest, CreateTransactionResponse>
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public CreateTransactionHandler(ITransactionRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<CreateTransactionResponse> Handle(CreateTransactionRequest request, CancellationToken cancellationToken)
        {
            request.Data.CreatedByUser = _authenticationProvider.User;

            var transactionEntity = _mapper.Map<Transaction>(request.Data);
            var savedTransaction = await _repository.AddAsync(transactionEntity);

            var addedRows = await _repository.SaveChangesAsync(cancellationToken);
            if (addedRows.IsNullOrDefault())
            {
                throw new SaveFailureException(nameof(transactionEntity), transactionEntity);
            }

            return new CreateTransactionResponse
            {
                       Data = _mapper.Map<TransactionDto>(savedTransaction),
                       ResponseType = eResponseType.Success
                   };
        }
    }
}