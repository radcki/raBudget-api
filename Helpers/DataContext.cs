using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PasswordChange> PasswordChanges { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Allocation> Allocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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