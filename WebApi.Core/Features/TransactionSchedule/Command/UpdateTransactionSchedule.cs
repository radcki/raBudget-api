using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Dto.User;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Features.TransactionSchedule.Command
{
    public class UpdateTransactionSchedule
    {
        public class Command : IRequest<TransactionScheduleDto>
        {
            public int TransactionScheduleId { get; set; }

            public string Description { get; set; }

            public double Amount { get; set; }

            public int BudgetCategoryId { get; set; }

            public eFrequency Frequency { get; set; }

            public int PeriodStep { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime? EndDate { get; set; }
            
        }


        public class UpdateTransactionScheduleRequestValidator : AbstractValidator<Command>
        {
            public UpdateTransactionScheduleRequestValidator()
            {
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.BudgetCategoryId).NotEmpty();
                RuleFor(x => x.Amount).NotEmpty();
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
                var transactionSchedule = await TransactionScheduleRepository.GetByIdAsync(request.TransactionScheduleId);
                if (transactionSchedule == null)
                {
                    throw new NotFoundException("Transaction schedule was not found.");
                }
                var sourceCategoryAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, transactionSchedule.Id);
                if (!await sourceCategoryAccessible)
                {
                    throw new NotFoundException("Source budget category was not found.");
                }

                transactionSchedule.Description = request.Description;
                transactionSchedule.StartDate = request.StartDate;
                transactionSchedule.Frequency = request.Frequency;
                transactionSchedule.PeriodStep = request.PeriodStep;
                transactionSchedule.EndDate = request.EndDate;
                transactionSchedule.BudgetCategoryId = request.BudgetCategoryId;

                await TransactionScheduleRepository.UpdateAsync(transactionSchedule);
                await TransactionScheduleRepository.SaveChangesAsync(cancellationToken);

                return Mapper.Map<TransactionScheduleDto>(transactionSchedule);
            }
        }
    }
}