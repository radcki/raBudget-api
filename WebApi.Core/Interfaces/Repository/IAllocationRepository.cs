using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Domain.Entities;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Interfaces.Repository
{
    public interface IAllocationRepository : IAsyncRepository<Allocation, int>
    {
        /// <summary>
        /// Find filtered transactions
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Collection of budget entities</returns>
        Task<IReadOnlyList<Allocation>> ListWithFilter(Budget budget, AllocationFilterModel filters);

      
    }
}