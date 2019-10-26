using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Handlers.TransactionScheduleHandlers.CreateTransactionSchedule;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers.DeleteTransactionSchedule
{
    /// <summary>
    /// Delete transactionSchedule by its id. In case of success, deleted transactionSchedule data is returned
    /// </summary>
    public class DeleteTransactionScheduleHandler : BaseTransactionScheduleHandler<DeleteTransactionScheduleRequest, TransactionScheduleDto>
    {
        public DeleteTransactionScheduleHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         ITransactionScheduleRepository transactionScheduleRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionScheduleRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<TransactionScheduleDto> Handle(DeleteTransactionScheduleRequest request, CancellationToken cancellationToken)
        {
            var transactionScheduleEntity = await TransactionScheduleRepository.GetByIdAsync(request.TransactionScheduleId);
            if (transactionScheduleEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, transactionScheduleEntity.BudgetCategoryId))
            {
                throw new NotFoundException("Target transaction schedule was not found.");
            }

            await TransactionScheduleRepository.DeleteAsync(transactionScheduleEntity);
            await TransactionScheduleRepository.SaveChangesAsync(cancellationToken);

            return Mapper.Map<TransactionScheduleDto>(transactionScheduleEntity);
        }
    }
}