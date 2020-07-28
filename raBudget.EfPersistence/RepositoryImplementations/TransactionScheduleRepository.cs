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
    public class TransactionScheduleRepository : ITransactionScheduleRepository
    {
        private readonly DataContext _db;

        public TransactionScheduleRepository(DataContext dataContext)
        {
            _db = dataContext;
        }

        #region Implementation of IAsyncRepository<TransactionSchedule,in int>

        /// <inheritdoc />
        public async Task<TransactionSchedule> GetByIdAsync(int id)
        {
            return await _db.TransactionSchedules.Include(x=>x.Transactions).FirstOrDefaultAsync(x=>x.Id == id);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TransactionSchedule>> ListAllAsync()
        {
            return await _db.TransactionSchedules.Include(x => x.Transactions).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<TransactionSchedule> AddAsync(TransactionSchedule entity)
        {
            await _db.TransactionSchedules.AddAsync(entity);
            return entity;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(TransactionSchedule entity)
        {
            await Task.FromResult(_db.Entry(entity).State = EntityState.Modified);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TransactionSchedule entity)
        {
            await Task.FromResult(_db.TransactionSchedules.Remove(entity));
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TransactionSchedule>> ListWithFilter(int budgetId, TransactionScheduleFilterModel filters)
        {
            var transactionSchedules = _db.TransactionSchedules
                                          .Include(x => x.BudgetCategory)
                                          .Include(x => x.CreatedByUser)
                                          .Where(x => x.BudgetCategory.BudgetId == budgetId);

            if (filters.TransactionScheduleIdFilter != null && filters.TransactionScheduleIdFilter.Any())
            {
                transactionSchedules = transactionSchedules.Where(x => filters.TransactionScheduleIdFilter.Contains(x.Id));
            }

            if (filters.CategoryIdFilter != null && filters.CategoryIdFilter.Any())
            {
                transactionSchedules = transactionSchedules.Where(x => filters.CategoryIdFilter.Contains(x.BudgetCategoryId));
            }

            if (filters.CreatedByUserIdFilter != null && filters.CreatedByUserIdFilter.Any())
            {
                transactionSchedules = transactionSchedules.Where(x => filters.CreatedByUserIdFilter.Contains(x.CreatedByUserId));
            }

            if (filters.StartDateStartFilter != null)
            {
                transactionSchedules = transactionSchedules.Where(x => x.StartDate >= filters.StartDateStartFilter);
            }

            if (filters.StartDateEndFilter != null)
            {
                transactionSchedules = transactionSchedules.Where(x => x.StartDate <= filters.StartDateEndFilter);
            }

            if (filters.EndDateStartFilter != null)
            { 
                transactionSchedules = transactionSchedules.Where(x => x.EndDate == null || x.EndDate >= filters.EndDateStartFilter);
            }

            if (filters.EndDateEndFilter != null)
            {
                transactionSchedules = transactionSchedules.Where(x => x.EndDate == null || x.EndDate <= filters.EndDateEndFilter);
            }

            if (filters.CategoryType != null)
            {
                transactionSchedules = transactionSchedules.Where(x => x.BudgetCategory.Type == filters.CategoryType);
            }

            if (filters.OrderBy != null)
            {
                switch (filters.DataOrder)
                {
                    case eDataOrder.Default:
                    case eDataOrder.Ascending:
                        transactionSchedules = transactionSchedules.OrderBy(x => filters.OrderBy(x));
                        break;
                    case eDataOrder.Descending:
                        transactionSchedules = transactionSchedules.OrderByDescending(x => filters.OrderBy(x));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }


            if (filters.LimitResults != null)
            {
                transactionSchedules = transactionSchedules.Take(filters.LimitResults.Value);
            }

            return await transactionSchedules.ToListAsync();
        }

        #endregion
    }
}