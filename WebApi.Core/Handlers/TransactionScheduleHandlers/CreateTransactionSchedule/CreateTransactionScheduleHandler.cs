using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers.CreateTransactionSchedule
{
    public class CreateTransactionScheduleHandler : BaseTransactionScheduleHandler<CreateTransactionScheduleRequest, TransactionScheduleDto>
    {
        public CreateTransactionScheduleHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         ITransactionScheduleRepository transactionRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<TransactionScheduleDto> Handle(CreateTransactionScheduleRequest request, CancellationToken cancellationToken)
        {
            if (!await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.BudgetCategoryId))
            {
                throw new NotFoundException("Target budget category was not found.");
            }

            request.Data.CreatedByUser = AuthenticationProvider.User;

            var transactionScheduleEntity = Mapper.Map<TransactionSchedule>(request.Data);
            transactionScheduleEntity.CreatedByUserId = AuthenticationProvider.User.UserId;

            var savedTransactionSchedule = await TransactionScheduleRepository.AddAsync(transactionScheduleEntity);

            var addedRows = await TransactionScheduleRepository.SaveChangesAsync(cancellationToken);
            if (addedRows.IsNullOrDefault())
            {
                throw new SaveFailureException(nameof(transactionScheduleEntity), transactionScheduleEntity);
            }

            return Mapper.Map<TransactionScheduleDto>(savedTransactionSchedule);
        }
    }
}