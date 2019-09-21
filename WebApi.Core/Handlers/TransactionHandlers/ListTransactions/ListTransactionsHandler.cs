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

namespace raBudget.Core.Handlers.TransactionHandlers.ListTransactions
{
    public class ListTransactionsHandler : BaseTransactionHandler<ListTransactionsRequest, IEnumerable<TransactionDto>>
    {

        public ListTransactionsHandler(IBudgetCategoryRepository budgetCategoryRepository,
                                       ITransactionRepository transactionRepository,
                                       IMapper mapper,
                                       IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<IEnumerable<TransactionDto>> Handle(ListTransactionsRequest request, CancellationToken cancellationToken)
        {
            var transactions = await TransactionRepository.ListWithFilter(Mapper.Map<Budget>(request.Budget), null);
            
            return Mapper.Map<IEnumerable<TransactionDetailsDto>>(transactions);
        }
    }
}