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
    public class BudgetCategoryRepository : IBudgetCategoryRepository
    {
        private readonly DataContext _db;

        public BudgetCategoryRepository(DataContext dataContext)
        {
            _db = dataContext;
        }

        #region Implementation of IAsyncRepository<Transaction,in int>

        /// <inheritdoc />
        public async Task<BudgetCategory> GetByIdAsync(int id)
        {
            return await _db.BudgetCategories.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<BudgetCategory>> ListAllAsync()
        {
            return await _db.BudgetCategories.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<BudgetCategory> AddAsync(BudgetCategory entity)
        {
            await _db.BudgetCategories.AddAsync(entity);
            return entity;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(BudgetCategory entity)
        {
            _db.BudgetCategoryBudgetedAmounts.RemoveRange(_db.BudgetCategoryBudgetedAmounts.Where(x => x.BudgetCategoryId == entity.Id));
            await Task.FromResult(_db.Entry(entity).State = EntityState.Modified);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(BudgetCategory entity)
        {
            await Task.FromResult(_db.BudgetCategories.Remove(entity));
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<BudgetCategory>> ListWithFilter(Budget budget, BudgetCategoryFilterModel filters)
        {
            var categories = _db.BudgetCategories
                                .AsNoTracking()
                                .Include(x=>x.Transactions)
                                .Include(x=>x.SourceAllocations)
                                .Include(x=>x.TargetAllocations)
                                .Include(x=>x.BudgetCategoryBudgetedAmounts)
                                .Where(x => x.BudgetId == budget.Id);

            /*--*/
            if (filters.CategoryIdFilter != null)
            {
                categories = categories.Where(x => filters.CategoryIdFilter.Any(s => s == x.Id));
            }


            if (filters.CategoryType != null)
            {
                categories = categories.Where(x => x.Type == filters.CategoryType);
            }

            /*--*/
            if (filters.OrderBy != null)
            {
                switch (filters.DataOrder)
                {
                    case eDataOrder.Default:
                    case eDataOrder.Ascending:
                        categories = categories.OrderBy(x => filters.OrderBy(x));
                        break;
                    case eDataOrder.Descending:
                        categories = categories.OrderByDescending(x => filters.OrderBy(x));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }


            return await categories.ToListAsync();
        }


        public async Task<bool> IsAccessibleToUser(User user, int budgetCategoryId)
        {
            return await IsAccessibleToUser(user.Id, budgetCategoryId);
        }

        public async Task<bool> IsAccessibleToUser(Guid userId, int budgetCategoryId)
        {
            return await _db.Budgets.AsNoTracking()
                            .Include(x => x.BudgetCategories)
                            .Include(x => x.BudgetShares)
                            .Where(x => x.OwnedByUserId == userId || x.BudgetShares.Any(s => s.SharedWithUserId == userId))
                            .SelectMany(x => x.BudgetCategories)
                            .AnyAsync(x => x.Id == budgetCategoryId);
        }

        #endregion
    }
}