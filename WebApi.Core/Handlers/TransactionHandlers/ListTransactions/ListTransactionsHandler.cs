using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Handlers.BudgetHandlers.ListAvailableBudgets;
using raBudget.Core.Infrastructure.AutoMapper;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Handlers.TransactionHandlers.ListTransactions
{
    public class ListTransactionsHandler : BaseTransactionHandler<ListTransactionsRequest, IEnumerable<TransactionDto>>
    {

        public ListTransactionsHandler(ITransactionRepository transactionRepository,
                                       IMapper mapper,
                                       IAuthenticationProvider authenticationProvider) : base(null, transactionRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<IEnumerable<TransactionDto>> Handle(ListTransactionsRequest request, CancellationToken cancellationToken)
        {
            if (request.Filters == null)
            {
                request.Filters = new TransactionFilterDto();
            }

            var filters = Mapper.Map<TransactionsFilterModel>(request.Filters);
            filters.OrderBy = x => x.CreationDateTime;
            var transactions = await TransactionRepository.ListWithFilter(Mapper.Map<Budget>(request.Budget), filters);
            
            return Mapper.Map<IEnumerable<TransactionDetailsDto>>(transactions);
        }
    }
}