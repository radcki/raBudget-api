using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Domain.Entities;

namespace raBudget.Core.Interfaces.Repository
{
    public interface IBudgetRepository : IAsyncRepository<Budget, int>
    {

        ///  /// <summary>
        /// Find all budgets available for user - owned and shared.
        /// </summary>
        /// <returns>Collection of budget entities</returns>
        Task<IEnumerable<Budget>> ListAvailableBudgets(Guid userId);


        /// <summary>
        /// Find budgets owned by user.
        /// </summary>
        /// <returns>Collection of budget entities</returns>
        Task<IEnumerable<Budget>> ListOwnedBudgets(Guid userId);

    }
}