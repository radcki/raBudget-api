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

namespace raBudget.Core.Handlers.TransactionSchedule.Command
{
    public class CreateTransactionSchedule
    {
        public class Command : IRequest<TransactionScheduleDto>
        {
            public TransactionScheduleDto Data;

            public Command(TransactionScheduleDto transaction)
            {
                Data = transaction;
            }
        }


        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Data.Description).NotEmpty();
                RuleFor(x => x.Data.Amount).NotEmpty();
                RuleFor(x => x.Data.BudgetCategoryId).NotEmpty();
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
                if (!await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.BudgetCategoryId))
                {
                    throw new NotFoundException("Target budget category was not found.");
                }

                request.Data.CreatedByUser = AuthenticationProvider.User;

                var transactionScheduleEntity = Mapper.Map<Domain.Entities.TransactionSchedule>(request.Data);
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
}