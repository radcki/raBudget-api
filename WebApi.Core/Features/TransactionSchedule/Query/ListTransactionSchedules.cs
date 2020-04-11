using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Features.TransactionSchedule.Query
{
    public class ListTransactionSchedules
    {
        public class Query : IRequest<IEnumerable<TransactionScheduleDto>>
        {
            public int BudgetId { get; set; }
            public TransactionScheduleFilterDto Filters { get; set; }

            public Query(int budgetId, TransactionScheduleFilterDto transactionScheduleFilter)
            {
                BudgetId = budgetId;
                Filters = transactionScheduleFilter;
            }
        }

        public class GetTransactionScheduleRequestValidator : AbstractValidator<Query>
        {
            public GetTransactionScheduleRequestValidator()
            {
                RuleFor(x => x.BudgetId).NotEmpty();
            }
        }

        public class Handler : BaseTransactionScheduleHandler<Query, IEnumerable<TransactionScheduleDto>>
        {
            public Handler
            (ITransactionScheduleRepository transactionRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(null, transactionRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<IEnumerable<TransactionScheduleDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var schedules = await TransactionScheduleRepository.ListWithFilter(request.BudgetId, Mapper.Map<TransactionScheduleFilterModel>(request.Filters));

                return Mapper.Map<IEnumerable<TransactionScheduleDto>>(schedules);
            }
        }
    }
}