using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace raBudget.EfPersistence.Configurations
{
    class BudgetShareConfiguration : IEntityTypeConfiguration<BudgetShare>
    { 
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<BudgetShare> builder)
        {
            // BudgetId
            builder.HasKey(f => f.BudgetShareId);
            builder.Property(f=>f.BudgetShareId).IsRequired().ValueGeneratedOnAdd();

            // Budget
            builder.HasIndex(f => f.BudgetId);
            builder.Property(f => f.BudgetId).IsRequired();
            builder.HasOne(f => f.Budget)
                   .WithMany(f => f.BudgetShares)
                   .OnDelete(DeleteBehavior.Cascade);

            // SharedWithUser
            builder.HasIndex(f => f.SharedWithUserId);
            builder.Property(f => f.SharedWithUserId).IsRequired();
            builder.HasOne(f => f.SharedWithUser)
                   .WithMany(f => f.BudgetShares)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
    
}
