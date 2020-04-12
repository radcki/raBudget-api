using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using raBudget.Core.Interfaces;
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
        private readonly IAuthenticationProvider _authenticationProvider;

        public BudgetCategoryRepository(DataContext dataContext, IAuthenticationProvider authenticationProvider)
        {
            _db = dataContext;
            _authenticationProvider = authenticationProvider;
        }

        #region Implementation of IAsyncRepository<Transaction,in int>

        /// <inheritdoc />
        public async Task<BudgetCategory> GetByIdAsync(int id)
        {
            return await _db.BudgetCategories
                            .Include(x => x.BudgetCategoryBudgetedAmounts)
                            .Where(x => AccessibleBudgetIds().Contains(x.BudgetId))
                            .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<BudgetCategory>> ListAllAsync()
        {
            return await _db.BudgetCategories
                            .Include(x=>x.BudgetCategoryBudgetedAmounts)
                            .Where(x=>AccessibleBudgetIds().Contains(x.BudgetId))
                            .OrderBy(x => x.Order)
                            .ToListAsync();
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
            entity.BudgetCategoryBudgetedAmounts = entity.BudgetCategoryBudgetedAmounts
                                                         .ToList()
                                                         .Select(x =>
                                                                 {
                                                                     var amount = new BudgetCategoryBudgetedAmount()
                                                                                  {
                                                                                      BudgetCategoryId = x.BudgetCategoryId,
                                                                                      MonthlyAmount = x.MonthlyAmount,
                                                                                      ValidFrom = x.ValidFrom,
                                                                                      ValidTo = x.ValidTo
                                                                                  };
                                                                     _db.Entry(amount).State = EntityState.Added;
                                                                     return amount;
                                                                 })
                                                         .ToList();

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
                                .Include(x => x.Transactions)
                                .Include(x => x.SourceAllocations)
                                .Include(x => x.TargetAllocations)
                                .Include(x => x.BudgetCategoryBudgetedAmounts)
                                .Where(x => AccessibleBudgetIds().Contains(x.BudgetId))
                                .OrderBy(x => x.Order)
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

        public async Task<bool> IsAccessibleToUser(int budgetCategoryId)
        {
            return await _db.Budgets
                            .AsNoTracking()
                            .Include(x => x.BudgetCategories)
                            .Where(x=>AccessibleBudgetIds().Contains(x.Id))
                            .SelectMany(x => x.BudgetCategories)
                            .AnyAsync(x => x.Id == budgetCategoryId);
        }

        private IQueryable<int> AccessibleBudgetIds()
        {
            return _db.Budgets
                      .Include(x => x.BudgetShares)
                      .Where(x => x.OwnedByUserId == _authenticationProvider.User.UserId
                                  || x.BudgetShares.Any(s => s.SharedWithUserId == _authenticationProvider.User.UserId))
                      .Select(x => x.Id);
        }

        #endregion
    }
}