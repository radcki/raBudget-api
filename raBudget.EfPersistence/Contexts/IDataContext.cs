using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using raBudget.Core.Interfaces;

namespace raBudget.EfPersistence.Contexts
{
    public interface IDataContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Budget> Budgets { get; set; }
        DbSet<Budget> Currencies { get; set; }
        DbSet<BudgetShare> BudgetShares { get; set; }
        DbSet<BudgetCategory> BudgetCategories { get; set; }
        DbSet<BudgetCategoryBudgetedAmount> BudgetCategoryBudgetedAmounts { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        DbSet<Allocation> Allocations { get; set; }
        DbSet<TransactionSchedule> TransactionSchedules { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}