using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Handlers.TransactionHandlers.CreateTransaction;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.TransactionHandlers.DeleteTransaction
{
    /// <summary>
    /// Delete transaction by its id. In case of success, deleted transaction data is returned
    /// </summary>
    public class DeleteTransactionHandler : BaseTransactionHandler<DeleteTransactionRequest, TransactionDto>
    {
        public DeleteTransactionHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         ITransactionRepository transactionRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<TransactionDto> Handle(DeleteTransactionRequest request, CancellationToken cancellationToken)
        {
            var transactionEntity = await TransactionRepository.GetByIdAsync(request.TransactionId);
            if (transactionEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, transactionEntity.BudgetCategoryId))
            {
                throw new NotFoundException("Target transaction was not found.");
            }

            await TransactionRepository.DeleteAsync(transactionEntity);
            await TransactionRepository.SaveChangesAsync(cancellationToken);

            return Mapper.Map<TransactionDto>(transactionEntity);
        }
    }
}