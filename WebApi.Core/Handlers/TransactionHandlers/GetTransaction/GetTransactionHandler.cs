using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Exceptions;
using raBudget.Core.ExtensionMethods;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Handlers.TransactionHandlers.DeleteTransaction;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.TransactionHandlers.GetTransaction
{
    public class GetTransactionHandler : BaseTransactionHandler<GetTransactionRequest, TransactionDetailsDto>
    {
        public GetTransactionHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         ITransactionRepository transactionRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<TransactionDetailsDto> Handle(GetTransactionRequest request, CancellationToken cancellationToken)
        {
            var transactionEntity = await TransactionRepository.GetByIdAsync(request.TransactionId);
            if (transactionEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, transactionEntity.Id))
            {
                throw new NotFoundException("Target transaction was not found.");
            }

            return Mapper.Map<TransactionDetailsDto>(transactionEntity);
        }
    }
}