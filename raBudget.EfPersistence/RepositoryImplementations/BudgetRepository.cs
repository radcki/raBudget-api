using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.EfPersistence.Contexts;

namespace raBudget.EfPersistence.RepositoryImplementations
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly DataContext _db;

        public BudgetRepository(DataContext dataContext)
        {
            _db = dataContext;
        }

        #region Implementation of IBudgetRepository

        /// <inheritdoc />
        public async Task<IEnumerable<Budget>> ListAvailableBudgets(Guid userId)
        {
            return await Task.FromResult(_db.Budgets
                                            .Include(x => x.BudgetShares)
                                            .Include(x=>x.OwnedByUser)
                                            .Include(x=>x.BudgetCategories).ThenInclude(x=>x.BudgetCategoryBudgetedAmounts)
                                            .Include(x => x.BudgetCategories).ThenInclude(x => x.Transactions)
                                            .Where(x => x.OwnedByUserId == userId
                                                        || x.BudgetShares.Any(s => s.SharedWithUserId == userId)));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Budget>> ListOwnedBudgets(Guid userId)
        {
            return await Task.FromResult(_db.Budgets
                                            .Include(x => x.BudgetShares)
                                            .Include(x=>x.OwnedByUser)
                                            .Include(x => x.BudgetCategories).ThenInclude(x => x.BudgetCategoryBudgetedAmounts)
                                            .Where(x => x.OwnedByUserId == userId));
        }

        /// <inheritdoc />
        public async Task<Budget> GetByIdAsync(int id)
        {
            return await _db.Budgets
                            .Include(x=>x.BudgetCategories).ThenInclude(x => x.BudgetCategoryBudgetedAmounts)
                            .Include(x=>x.BudgetShares).ThenInclude(x=>x.SharedWithUser)
                            .Include(x=>x.OwnedByUser)
                            .FirstOrDefaultAsync(x=>x.Id == id);
        }
        /// <inheritdoc />
        public async Task<IReadOnlyList<Budget>> ListAllAsync()
        {
            return await _db.Budgets.AsNoTracking().ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Budget> AddAsync(Budget entity)
        {
            await _db.Budgets.AddAsync(entity);
            return entity;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Budget entity)
        {
            await Task.FromResult(_db.Entry(entity).State = EntityState.Modified);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Budget entity)
        {
            await Task.FromResult(_db.Budgets.Remove(entity));
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }

        #endregion

    }
}