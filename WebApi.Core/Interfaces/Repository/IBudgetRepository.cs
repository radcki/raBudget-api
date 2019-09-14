using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Domain.Entities;

namespace raBudget.Core.Interfaces.Repository
{
    public interface IBudgetRepository<T> : IAsyncRepository<T, int> where T : Budget
    {
        /// <summary>
        /// Find all budgets available for user - owned and shared.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Collection of budget entities</returns>
        Task<IEnumerable<Budget>> ListAvailableBudgets(User user);

        /// <summary>
        /// Find budgets owned by user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Collection of budget entities</returns>
        Task<IEnumerable<Budget>> ListOwnedBudgets(User user);
    }
}