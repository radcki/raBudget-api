using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Domain.Entities;

namespace raBudget.Core.Interfaces.Repository
{
    public interface IUserRepository<T> : IAsyncRepository<T, Guid> where T : User
    {
    }
}