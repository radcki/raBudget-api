using System;
using System.Collections.Generic;
using System.Linq;
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
        #region Privates

        private readonly DataContext _db;

        #endregion

        #region Constructors

        public TransactionRepository(DataContext dataContext)
        {
            _db = dataContext;
        }

        #endregion

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
                                  .Where(x => x.BudgetCategory.BudgetId == budget.Id);

            /*--*/
            if (filters.TransactionIdFilter != null && filters.TransactionIdFilter.Any())
            {
                transactions = transactions.Where(x => filters.TransactionIdFilter.Any(s => s == x.Id));
            }

            /*--*/
            if (filters.CategoryIdFilter != null && filters.CategoryIdFilter.Any())
            {
                transactions = transactions.Where(x => filters.CategoryIdFilter.Any(s => s == x.BudgetCategoryId));
            }

            /*--*/
            if (filters.TransactionScheduleIdFilter != null && filters.TransactionScheduleIdFilter.Any())
            {
                transactions = transactions.Where(x => filters.TransactionScheduleIdFilter.Any(s => s == x.TransactionScheduleId));
            }

            /*--*/
            if (filters.CategoryType != null)
            {
                transactions = transactions.Where(x => x.BudgetCategory.Type == filters.CategoryType);
            }

            /*--*/
            if (filters.CreatedByUserIdFilter != null && filters.CreatedByUserIdFilter.Any())
            {
                transactions = transactions.Where(x => filters.CreatedByUserIdFilter.Any(s => s == x.CreatedByUserId));
            }

            /*--*/
            if (filters.CreationDateEndFilter != null)
            {
                transactions = transactions.Where(x => x.CreationDateTime.Date <= filters.CreationDateEndFilter.Value.Date);
            }

            if (filters.CreationDateStartFilter != null)
            {
                transactions = transactions.Where(x => x.CreationDateTime.Date >= filters.CreationDateStartFilter.Value.Date);
            }

            /*--*/
            if (filters.TransactionDateEndFilter != null)
            {
                transactions = transactions.Where(x => x.TransactionDateTime.Date <= filters.TransactionDateEndFilter.Value.Date);
            }

            if (filters.TransactionDateStartFilter != null)
            {
                transactions = transactions.Where(x => x.TransactionDateTime.Date >= filters.TransactionDateStartFilter.Value.Date);
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


            if (filters.LimitCategoryTypeResults != null)
            {
                var income = transactions.Where(x => x.BudgetCategory.Type == eBudgetCategoryType.Income)
                                         .Take(filters.LimitCategoryTypeResults.Value);
                var saving = transactions.Where(x => x.BudgetCategory.Type == eBudgetCategoryType.Saving)
                                         .Take(filters.LimitCategoryTypeResults.Value);
                var spending = transactions.Where(x => x.BudgetCategory.Type == eBudgetCategoryType.Spending)
                                           .Take(filters.LimitCategoryTypeResults.Value);

                transactions = spending.Union(income).Union(saving);
            }

            return await transactions.Include(x => x.BudgetCategory).ToListAsync();
        }

        #endregion
    }
}