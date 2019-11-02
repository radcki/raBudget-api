using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers.UpdateTransactionSchedule
{
    public class UpdateTransactionScheduleHandler : BaseTransactionScheduleHandler<UpdateTransactionScheduleRequest, TransactionScheduleDto>
    {
        public UpdateTransactionScheduleHandler(IBudgetCategoryRepository budgetCategoryRepository,
                                        ITransactionScheduleRepository transactionRepository,
                                        IMapper mapper,
                                        IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<TransactionScheduleDto> Handle(UpdateTransactionScheduleRequest request, CancellationToken cancellationToken)
        {
            var transaction = await TransactionScheduleRepository.GetByIdAsync(request.Data.TransactionScheduleId);
            var sourceCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, transaction.Id);
            if (!await sourceCategoryAccessible)
            {
                throw new NotFoundException("Source budget category was not found.");
            }

            transaction.Description = request.Data.Description;
            transaction.StartDate = request.Data.StartDate;
            transaction.EndDate = request.Data.EndDate;
            transaction.BudgetCategoryId = request.Data.BudgetCategoryId;

            await TransactionScheduleRepository.UpdateAsync(transaction);
            await TransactionScheduleRepository.SaveChangesAsync(cancellationToken);

            return Mapper.Map<TransactionScheduleDto>(transaction);
        }
    }
}