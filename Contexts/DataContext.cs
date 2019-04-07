using Microsoft.EntityFrameworkCore;
using WebApi.Models.Entities;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace WebApi.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
        public DbSet<Log> Logs { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<PasswordChange> PasswordChanges { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }
        public DbSet<BudgetCategoryAmountConfig> BudgetCategoryAmountConfigs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Allocation> Allocations { get; set; }
        public DbSet<TransactionSchedule> TransactionSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            LogModelBuilderHelper.Build(modelBuilder.Entity<Log>());
            modelBuilder.Entity<Log>().ToTable("Log");

            modelBuilder.Entity<Allocation>().Property(c => c.AllocationId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Budget>().Property(c => c.BudgetId).ValueGeneratedOnAdd();
            modelBuilder.Entity<BudgetCategory>().Property(c => c.BudgetCategoryId).ValueGeneratedOnAdd();
            modelBuilder.Entity<BudgetCategoryAmountConfig>().Property(c => c.BudgetCategoryAmountConfigId).ValueGeneratedOnAdd();
            modelBuilder.Entity<PasswordChange>().Property(c => c.PasswordChangeId).ValueGeneratedOnAdd();
            modelBuilder.Entity<PasswordReset>().Property(c => c.PasswordResetId).ValueGeneratedOnAdd();
            modelBuilder.Entity<RefreshToken>().Property(c => c.RefreshTokenId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Transaction>().Property(c => c.TransactionId).ValueGeneratedOnAdd();
            modelBuilder.Entity<TransactionSchedule>().Property(c => c.TransactionScheduleId).ValueGeneratedOnAdd();
            modelBuilder.Entity<User>().Property(c => c.UserId).ValueGeneratedOnAdd();
            modelBuilder.Entity<UserRole>().Property(c => c.UserRoleId).ValueGeneratedOnAdd();

            modelBuilder.Entity<User>().Property(b => b.Password).IsRequired();
            modelBuilder.Entity<User>().Property(b => b.Username).IsRequired();
            modelBuilder.Entity<User>().Property(b => b.Email).IsRequired();

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<RefreshToken>().HasIndex(u => u.ClientId).IsUnique();
            modelBuilder.Entity<RefreshToken>().Property(b => b.ClientId).IsRequired();
            modelBuilder.Entity<RefreshToken>().Property(b => b.Token).IsRequired();
            modelBuilder.Entity<RefreshToken>().Property(b => b.UserId).IsRequired();


            modelBuilder.Entity<UserRole>().Property(b => b.UserId).IsRequired();

            modelBuilder.Entity<Budget>().Property(b => b.Name).IsRequired();
            modelBuilder.Entity<Budget>().Property(b => b.UserId).IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}