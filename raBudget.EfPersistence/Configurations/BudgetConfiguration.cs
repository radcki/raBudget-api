using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace raBudget.EfPersistence.Configurations
{
    class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    { 
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            // BudgetId
            builder.HasKey(f => f.Id);
            builder.Property(f=>f.Id).IsRequired().ValueGeneratedOnAdd();

            // Name
            builder.Property(f => f.Name).IsRequired();

            // OwnedByUser
            builder.HasIndex(f => f.OwnedByUserId);
            builder.Property(f => f.OwnedByUserId).IsRequired();
            builder.HasOne(f => f.OwnedByUser)
                   .WithMany(f => f.OwnedBudgets)
                   .OnDelete(DeleteBehavior.Cascade);

            // StartingDate
            builder.Property(f => f.StartingDate).IsRequired();

            // Currency
            builder.Property(f => f.CurrencyCode).IsRequired();
            builder.Ignore(x => x.Currency);

            // Business logic
            builder.Ignore(x => x.IncomeCategoriesBalance);
            builder.Ignore(x => x.SpendingCategoriesBalance);
            builder.Ignore(x => x.SavingCategoriesBalance);
        }
    }
    
}
