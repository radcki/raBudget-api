using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace raBudget.EfPersistence.Configurations
{
    class BudgetCategoryBudgetedAmountConfiguration : IEntityTypeConfiguration<BudgetCategoryBudgetedAmount>
    { 
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<BudgetCategoryBudgetedAmount> builder)
        {
            // BudgetCategoryId
            builder.HasKey(f => f.BudgetCategoryBudgetedAmountId);
            builder.Property(f=>f.BudgetCategoryBudgetedAmountId).IsRequired().ValueGeneratedOnAdd();

            // MonthlyAmount
            builder.Property(f => f.MonthlyAmount).IsRequired();

            // ValidFrom
            builder.Property(f => f.ValidFrom).IsRequired();

            // Budget
            builder.HasIndex(f => f.BudgetCategoryId);
            builder.Property(f => f.BudgetCategoryId).IsRequired();
            builder.HasOne(f => f.BudgetCategory)
                   .WithMany(f => f.BudgetCategoryBudgetedAmounts)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
}
