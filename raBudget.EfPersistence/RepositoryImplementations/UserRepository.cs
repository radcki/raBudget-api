using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Core.Dto.User.Response;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.EfPersistence.Contexts;

namespace raBudget.EfPersistence.RepositoryImplementations
{
    public class UserRepository : IUserRepository<User>
    {
        private readonly DataContext _db;

        public UserRepository(DataContext dataContext)
        {
            _db = dataContext;
        }
       
        #region Implementation of IRepositoryBase

        /// <inheritdoc />
        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _db.Users.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<User>> ListAllAsync()
        {
            return await _db.Users.AsNoTracking().ToListAsync();
        }

        /// <inheritdoc />
        public async Task<User> AddAsync(User entity)
        {
            await _db.Users.AddAsync(entity);
            return entity;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(User entity)
        {
            await Task.FromResult(_db.Entry(entity).State = EntityState.Modified);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(User entity)
        {
            await Task.FromResult(_db.Users.Remove(entity));
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }

        #endregion
    }
}
