using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.TransactionHandlers.UpdateTransaction
{
    public class UpdateTransactionHandler : BaseTransactionHandler<UpdateTransactionRequest, TransactionDetailsDto>
    {
        public UpdateTransactionHandler(IBudgetCategoryRepository budgetCategoryRepository,
                                        ITransactionRepository transactionRepository,
                                        IMapper mapper,
                                        IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<TransactionDetailsDto> Handle(UpdateTransactionRequest request, CancellationToken cancellationToken)
        {
            var transaction = await TransactionRepository.GetByIdAsync(request.Data.TransactionId);
            var budgetCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.BudgetCategoryId);
            if (!await budgetCategoryAccessible)
            {
                throw new NotFoundException("Target budget category was not found.");
            }

            transaction.Description = request.Data.Description;
            transaction.Amount = request.Data.Amount;
            transaction.TransactionDateTime = request.Data.TransactionDate;
            transaction.BudgetCategoryId = request.Data.BudgetCategoryId;
            transaction.TransactionScheduleId = request.Data.TransactionSchedule?.TransactionScheduleId;

            await TransactionRepository.UpdateAsync(transaction);
            await TransactionRepository.SaveChangesAsync(cancellationToken);

            return Mapper.Map<TransactionDetailsDto>(transaction);
        }
    }
}