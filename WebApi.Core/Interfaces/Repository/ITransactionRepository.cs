﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Domain.Entities;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Interfaces.Repository
{
    public interface ITransactionRepository : IAsyncRepository<Transaction, int>
    {
        /// <summary>
        /// Find filtered transactions
        /// </summary>
        /// <param name="budget"></param>
        /// <param name="filters"></param>
        /// <returns>Collection of budget entities</returns>
        Task<IReadOnlyList<Transaction>> ListWithFilter(Budget budget, TransactionsFilterModel filters);

      
    }
}