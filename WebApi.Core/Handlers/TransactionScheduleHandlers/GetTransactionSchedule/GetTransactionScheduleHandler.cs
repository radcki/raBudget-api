using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Handlers.TransactionScheduleHandlers.DeleteTransactionSchedule;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers.GetTransactionSchedule
{
    public class GetTransactionScheduleHandler : BaseTransactionScheduleHandler<GetTransactionScheduleRequest, TransactionScheduleDetailsDto>
    {
        public GetTransactionScheduleHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         ITransactionScheduleRepository transactionScheduleRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionScheduleRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<TransactionScheduleDetailsDto> Handle(GetTransactionScheduleRequest request, CancellationToken cancellationToken)
        {
            var transactionScheduleEntity = await TransactionScheduleRepository.GetByIdAsync(request.TransactionScheduleId);
            if (transactionScheduleEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, transactionScheduleEntity.Id))
            {
                throw new NotFoundException("Target Transaction Schedule was not found.");
            }

            return Mapper.Map<TransactionScheduleDetailsDto>(transactionScheduleEntity);
        }
    }
}