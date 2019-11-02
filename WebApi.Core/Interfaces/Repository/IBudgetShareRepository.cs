using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Domain.Entities;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Interfaces.Repository
{
    public interface IBudgetShareRepository : IAsyncRepository<BudgetShare, int>
    {
        Task<IReadOnlyList<BudgetShare>> ListWithFilter(Budget budget, BudgetShareFilterModel filters);
    }
}