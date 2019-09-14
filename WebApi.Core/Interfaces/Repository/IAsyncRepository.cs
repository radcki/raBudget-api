using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using raBudget.Domain.Entities;

namespace raBudget.Core.Interfaces.Repository
{
    public interface IAsyncRepository<T, in TKeyType> where T : BaseEntity<TKeyType>
    {
        Task<T> GetByIdAsync(TKeyType id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
