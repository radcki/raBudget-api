using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Features.TransactionSchedule.Command
{
    public class DeleteTransactionSchedule
    {
        public class Command : IRequest<TransactionScheduleDto>
        {
            public int TransactionScheduleId;

            public Command(int transactionScheduleId)
            {
                TransactionScheduleId = transactionScheduleId;
            }
        }

        public class DeleteTransactionScheduleRequestValidator : AbstractValidator<Command>
        {
            public DeleteTransactionScheduleRequestValidator()
            {
                RuleFor(x => x.TransactionScheduleId).NotEmpty();
            }
        }

        /// <summary>
        /// Delete transactionSchedule by its id. In case of success, deleted transactionSchedule data is returned
        /// </summary>
        public class Handler : BaseTransactionScheduleHandler<Command, TransactionScheduleDto>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             ITransactionScheduleRepository transactionScheduleRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionScheduleRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<TransactionScheduleDto> Handle(Command request, CancellationToken cancellationToken)
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
}