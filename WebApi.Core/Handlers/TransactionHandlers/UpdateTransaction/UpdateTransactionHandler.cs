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
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public UpdateTransactionHandler(IBudgetCategoryRepository budgetCategoryRepository,
                                        ITransactionRepository transactionRepository,
                                        IMapper mapper,
                                        IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<TransactionDetailsDto> Handle(UpdateTransactionRequest request, CancellationToken cancellationToken)
        {
            var transaction = await TransactionRepository.GetByIdAsync(request.Data.TransactionId);
            var sourceCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, transaction.Id);
            var targetCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.TransactionId);
            if (!await sourceCategoryAccessible)
            {
                throw new NotFoundException("Source budget category was not found.");
            }
            if (!await targetCategoryAccessible)
            {
                throw new NotFoundException("Target budget category was not found.");
            }

            transaction.Description = request.Data.Description;
            transaction.TransactionDateTime = request.Data.TransactionDate;
            transaction.BudgetCategoryId = request.Data.BudgetCategoryId;
            transaction.TransactionScheduleId = request.Data.TransactionSchedule.TransactionScheduleId;

            await TransactionRepository.UpdateAsync(transaction);
            await TransactionRepository.SaveChangesAsync(cancellationToken);

            return Mapper.Map<TransactionDetailsDto>(transaction);
        }
    }
}