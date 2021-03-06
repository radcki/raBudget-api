﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.EfPersistence.Contexts;

namespace raBudget.EfPersistence.RepositoryImplementations
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly DataContext _db;
        private readonly IAuthenticationProvider _authenticationProvider;

        public BudgetRepository(DataContext dataContext, IAuthenticationProvider authenticationProvider)
        {
            _db = dataContext;
            _authenticationProvider = authenticationProvider;
        }

        #region Implementation of IBudgetRepository

        /// <inheritdoc />
        public async Task<IEnumerable<Budget>> ListAvailableBudgets()
        {
            return await Task.FromResult(_db.Budgets
                                            .Include(x => x.BudgetShares)
                                            .Include(x => x.OwnedByUser)
                                            .Include(x => x.BudgetCategories)
                                            .ThenInclude(x => x.BudgetCategoryBudgetedAmounts)
                                            .Include(x => x.BudgetCategories)
                                            .ThenInclude(x => x.Transactions)
                                            .Include(x => x.BudgetCategories)
                                            .ThenInclude(x => x.SourceAllocations)
                                            .Include(x => x.BudgetCategories)
                                            .ThenInclude(x => x.TargetAllocations)
                                            .Where(x => x.OwnedByUserId == _authenticationProvider.User.UserId
                                                        || x.BudgetShares.Any(s => s.SharedWithUserId == _authenticationProvider.User.UserId)));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Budget>> ListOwnedBudgets()
        {
            return await Task.FromResult(_db.Budgets
                                            .Include(x => x.BudgetShares)
                                            .Include(x => x.OwnedByUser)
                                            .Include(x => x.BudgetCategories)
                                            .ThenInclude(x => x.BudgetCategoryBudgetedAmounts)
                                            .Where(x => x.OwnedByUserId == _authenticationProvider.User.UserId));
        }

        /// <inheritdoc />
        public async Task<Budget> GetByIdAsync(int id)
        {
            var budget = await _db.Budgets
                                  .Include(x => x.BudgetCategories)
                                  .ThenInclude(x => x.BudgetCategoryBudgetedAmounts)
                                  .Include(x => x.BudgetShares)
                                  .ThenInclude(x => x.SharedWithUser)
                                  .Include(x => x.BudgetCategories)
                                  .ThenInclude(x => x.Transactions)
                                  .Include(x => x.BudgetCategories)
                                  .ThenInclude(x => x.SourceAllocations)
                                  .Include(x => x.BudgetCategories)
                                  .ThenInclude(x => x.TargetAllocations)
                                  .Include(x => x.OwnedByUser)
                                  .FirstOrDefaultAsync(x => x.Id == id);
            budget.BudgetCategories = budget.BudgetCategories.OrderBy(x => x.Order).ToList();
            return budget;
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


        /// <inheritdoc />
        public Task<bool> IsAccessibleToUser(int budgetId)
        {
            return _db.Budgets.AnyAsync(x => x.Id == budgetId && x.OwnedByUserId == _authenticationProvider.User.UserId || x.BudgetShares.Any(s => s.SharedWithUserId == _authenticationProvider.User.UserId));
        }

        #endregion
    }
}