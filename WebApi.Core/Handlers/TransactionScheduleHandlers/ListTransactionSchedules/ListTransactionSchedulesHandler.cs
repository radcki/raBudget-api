using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Handlers.TransactionScheduleHandlers.ListTransactionSchedules
{
    public class ListTransactionSchedulesHandler : BaseTransactionScheduleHandler<ListTransactionSchedulesRequest, IEnumerable<TransactionScheduleDto>>
    {

        public ListTransactionSchedulesHandler(ITransactionScheduleRepository transactionRepository,
                                       IMapper mapper,
                                       IAuthenticationProvider authenticationProvider) : base(null, transactionRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<IEnumerable<TransactionScheduleDto>> Handle(ListTransactionSchedulesRequest request, CancellationToken cancellationToken)
        {

            var schedules = await TransactionScheduleRepository.ListWithFilter(Mapper.Map<Budget>(request.Budget), Mapper.Map<TransactionScheduleFilterModel>(request.Filters));
            
            return Mapper.Map<IEnumerable<TransactionScheduleDto>>(schedules);
        }
    }
}