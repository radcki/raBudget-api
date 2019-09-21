using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.Domain.FilterModels;
using raBudget.EfPersistence.Contexts;

namespace raBudget.EfPersistence.RepositoryImplementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DataContext _db;

        public TransactionRepository(DataContext dataContext)
        {
            _db = dataContext;
        }

        #region Implementation of IAsyncRepository<Transaction,in int>

        /// <inheritdoc />
        public async Task<Transaction> GetByIdAsync(int id)
        {
            return await _db.Transactions.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Transaction>> ListAllAsync()
        {
            return await _db.Transactions.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Transaction> AddAsync(Transaction entity)
        {
            await _db.Transactions.AddAsync(entity);
            return entity;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Transaction entity)
        {
            await Task.FromResult(_db.Entry(entity).State = EntityState.Modified);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Transaction entity)
        {
            await Task.FromResult(_db.Transactions.Remove(entity));
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Transaction>> ListWithFilter(Budget budget, TransactionsFilterModel filters)
        {
            var transactions = _db.Transactions
                                  .AsNoTracking()
                                  .Include(x => x.BudgetCategory)
                                  .Where(x => x.BudgetCategory.BudgetId == budget.Id);

            /*--*/
            if (filters.TransactionIdFilter != null)
            {
                transactions = transactions.Where(x => filters.TransactionIdFilter.Any(s => s == x.Id));
            }

            /*--*/
            if (filters.CategoryIdFilter != null)
            {
                transactions = transactions.Where(x => filters.CategoryIdFilter.Any(s => s == x.BudgetCategoryId));
            }

            /*--*/
            if (filters.TransactionScheduleIdFilter != null)
            {
                transactions = transactions.Where(x => filters.TransactionScheduleIdFilter.Any(s => s == x.TransactionScheduleId));
            }

            /*--*/
            if (filters.CategoryType != null)
            {
                transactions = transactions.Where(x => x.BudgetCategory.Type == filters.CategoryType);
            }

            /*--*/
            if (filters.CreatedByUserIdFilter != null)
            {
                transactions = transactions.Where(x => filters.CreatedByUserIdFilter.Any(s => s == x.CreatedByUserId));
            }

            /*--*/
            if (filters.CreationDateEndFilter != null)
            {
                transactions = transactions.Where(x => x.CreationDateTime <= filters.CreationDateEndFilter);
            }
            if (filters.CreationDateStartFilter != null)
            {
                transactions = transactions.Where(x => x.CreationDateTime >= filters.CreationDateStartFilter);
            }

            /*--*/
            if (filters.TransactionDateEndFilter != null)
            {
                transactions = transactions.Where(x => x.TransactionDateTime <= filters.TransactionDateStartFilter);
            }
            if (filters.TransactionDateStartFilter != null)
            {
                transactions = transactions.Where(x => x.TransactionDateTime >= filters.TransactionDateStartFilter);
            }

            if (filters.CategoryType != null)
            {
                transactions = transactions.Where(x => x.BudgetCategory.Type == filters.CategoryType);
            }

            /*--*/
            if (filters.OrderBy != null)
            {
                switch (filters.DataOrder)
                {
                    case eDataOrder.Default:
                    case eDataOrder.Ascending:
                        transactions = transactions.OrderBy(x => filters.OrderBy(x));
                        break;
                    case eDataOrder.Descending:
                        transactions = transactions.OrderByDescending(x => filters.OrderBy(x));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }


            if (filters.LimitResults != null)
            {
                transactions = transactions.Take(filters.LimitResults.Value);
            }

            return await transactions.ToListAsync();

        }

        

        #endregion
    }
}