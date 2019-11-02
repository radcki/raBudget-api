using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Domain.Entities;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Interfaces.Repository
{
    public interface IBudgetCategoryRepository : IAsyncRepository<BudgetCategory, int>
    {
        Task<IReadOnlyList<BudgetCategory>> ListWithFilter(Budget budget, BudgetCategoryFilterModel filters);

        Task<bool> IsAccessibleToUser(User user, int budgetCategoryId);
        Task<bool> IsAccessibleToUser(Guid userId, int budgetCategoryId);
    }
}