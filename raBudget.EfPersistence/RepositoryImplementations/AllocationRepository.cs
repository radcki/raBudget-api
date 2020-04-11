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
    public class AllocationRepository : IAllocationRepository
    {
        private readonly DataContext _db;

        public AllocationRepository(DataContext dataContext)
        {
            _db = dataContext;
        }

        #region Implementation of IAsyncRepository<Allocation,in int>

        /// <inheritdoc />
        public async Task<Allocation> GetByIdAsync(int id)
        {
            return await _db.Allocations.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Allocation>> ListAllAsync()
        {
            return await _db.Allocations.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Allocation> AddAsync(Allocation entity)
        {
            await _db.Allocations.AddAsync(entity);
            return entity;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Allocation entity)
        {
            await Task.FromResult(_db.Entry(entity).State = EntityState.Modified);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Allocation entity)
        {
            await Task.FromResult(_db.Allocations.Remove(entity));
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Allocation>> ListWithFilter(Budget budget, AllocationFilterModel filters)
        {
            var allocations = _db.Allocations
                                 .AsNoTracking()
                                 .Include(x => x.TargetBudgetCategory)
                                 .Include(x => x.SourceBudgetCategory)
                                 .Where(x => x.TargetBudgetCategory.BudgetId == budget.Id);

            /*--*/
            if (filters.AllocationIdFilter != null && filters.AllocationIdFilter.Any())
            {
                allocations = allocations.Where(x => filters.AllocationIdFilter.Any(s => s == x.Id));
            }

            /*--*/
            if (filters.TargetCategoryIdFilter != null && filters.TargetCategoryIdFilter.Any())
            {
                allocations = allocations.Where(x => filters.TargetCategoryIdFilter.Any(s => s == x.TargetBudgetCategoryId));
            }

            if (filters.SourceCategoryIdFilter != null && filters.SourceCategoryIdFilter.Any())
            {
                allocations = allocations.Where(x => filters.SourceCategoryIdFilter.Any(s => s == x.SourceBudgetCategoryId));
            }

            /*--*/
            if (filters.CreatedByUserIdFilter != null && filters.CreatedByUserIdFilter.Any())
            {
                allocations = allocations.Where(x => filters.CreatedByUserIdFilter.Any(s => s == x.CreatedByUserId));
            }

            /*--*/
            if (filters.CreationDateEndFilter != null)
            {
                allocations = allocations.Where(x => x.CreationDateTime.Date <= filters.CreationDateEndFilter.Value.Date);
            }

            if (filters.CreationDateStartFilter != null)
            {
                allocations = allocations.Where(x => x.CreationDateTime.Date >= filters.CreationDateStartFilter.Value.Date);
            }

            /*--*/
            if (filters.AllocationDateEndFilter != null)
            {
                allocations = allocations.Where(x => x.AllocationDateTime.Date <= filters.AllocationDateEndFilter.Value.Date);
            }

            if (filters.AllocationDateStartFilter != null)
            {
                allocations = allocations.Where(x => x.AllocationDateTime.Date >= filters.AllocationDateStartFilter.Value.Date);
            }

            /*--*/
            if (filters.OrderBy != null)
            {
                switch (filters.DataOrder)
                {
                    case eDataOrder.Default:
                    case eDataOrder.Ascending:
                        allocations = allocations.OrderBy(x => filters.OrderBy(x));
                        break;
                    case eDataOrder.Descending:
                        allocations = allocations.OrderByDescending(x => filters.OrderBy(x));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }


            if (filters.LimitResults != null)
            {
                allocations = allocations.Take(filters.LimitResults.Value);
            }

            return await allocations.ToListAsync();
        }

        #endregion
    }
}