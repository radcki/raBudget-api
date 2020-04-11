using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.TransactionSchedule.Command
{
    public class UpdateTransactionSchedule
    {
        public class Command : IRequest<TransactionScheduleDto>
        {
            public TransactionScheduleDetailsDto Data;

            public Command(TransactionScheduleDetailsDto transactionSchedule)
            {
                Data = transactionSchedule;
            }
        }


        public class UpdateTransactionScheduleRequestValidator : AbstractValidator<Command>
        {
            public UpdateTransactionScheduleRequestValidator()
            {
                RuleFor(x => x.Data.Description).NotEmpty();
                RuleFor(x => x.Data.BudgetCategoryId).NotEmpty();
                RuleFor(x => x.Data.Amount).NotEmpty();
            }
        }

        public class Handler : BaseTransactionScheduleHandler<Command, TransactionScheduleDto>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             ITransactionScheduleRepository transactionRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<TransactionScheduleDto> Handle(Command request, CancellationToken cancellationToken)
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
}