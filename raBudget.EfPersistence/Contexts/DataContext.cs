using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using raBudget.Core.Interfaces;

namespace raBudget.EfPersistence.Contexts
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Budget> Currencies { get; set; }

        public DbSet<BudgetShare> BudgetShares { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }
        public DbSet<BudgetCategoryBudgetedAmount> BudgetCategoryBudgetedAmounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Allocation> Allocations { get; set; }
        public DbSet<TransactionSchedule> TransactionSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}