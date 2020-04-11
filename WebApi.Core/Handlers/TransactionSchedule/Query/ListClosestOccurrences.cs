using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Handlers.TransactionSchedule.Query
{
    public class ListClosestOccurrences
    {
        public class Query : IRequest<IEnumerable<TransactionDto>>
        {
            public BudgetDto Budget { get; set; }
            public int DaysForwardLimit { get; set; }
            public int DaysBackwardLimit { get; set; }

            public Query(BudgetDto budget, int daysBackwardLimit, int daysForwardLimit)
            {
                Budget = budget;
                DaysForwardLimit = daysForwardLimit;
                DaysBackwardLimit = daysBackwardLimit;
            }
        }

        public class GetTransactionScheduleRequestValidator : AbstractValidator<Query>
        {
            public GetTransactionScheduleRequestValidator()
            {
                RuleFor(x => x.Budget).NotEmpty();
                RuleFor(x => x.Budget.BudgetId).NotEmpty();
            }
        }


        public class Handler : BaseTransactionScheduleHandler<Query, IEnumerable<TransactionDto>>
        {
            public Handler
            (ITransactionScheduleRepository transactionRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(null, transactionRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<IEnumerable<TransactionDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var schedules = await TransactionScheduleRepository.ListWithFilter(Mapper.Map<Budget>(request.Budget), new TransactionScheduleFilterModel()
                                                                                                                       {
                                                                                                                           StartDateEndFilter = DateTime.Today.AddDays(1),
                                                                                                                           EndDateStartFilter = DateTime.Today
                                                                                                                       });
                var occurrences = new List<TransactionDto>();
                var startDate = DateTime.Today.AddDays(-request.DaysBackwardLimit);
                var endDate = DateTime.Today.AddDays(request.DaysForwardLimit);
                foreach (var schedule in schedules)
                {
                    var occurrenceDates = schedule.OccurrencesInPeriod(startDate, endDate);
                    var alreadyCreatedTransactionDates = (await TransactionScheduleRepository
                                                             .GetByIdAsync(schedule.Id))
                                                        .Transactions
                                                        .Where(x => x.TransactionDateTime.Date >= startDate && x.TransactionDateTime.Date <= endDate)
                                                        .Select(x => x.TransactionDateTime)
                                                        .ToList();

                    occurrenceDates.Except(alreadyCreatedTransactionDates)
                                   .ToList()
                                   .ForEach(date =>
                                            {
                                                occurrences.Add(new TransactionDto()
                                                                {
                                                                    BudgetCategoryId = schedule.BudgetCategoryId,
                                                                    Amount = schedule.Amount,
                                                                    TransactionDate = date,
                                                                    Description = schedule.Description,
                                                                    TransactionScheduleId = schedule.Id
                                                                });
                                            });
                }

                return occurrences.OrderBy(x => x.TransactionDate);
            }
        }
    }
}