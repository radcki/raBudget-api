using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace raBudget.EfPersistence.Configurations
{
    class BudgetCategoryConfiguration : IEntityTypeConfiguration<BudgetCategory>
    { 
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<BudgetCategory> builder)
        {
            // BudgetCategoryId
            builder.HasKey(f => f.Id);
            builder.Property(f=>f.Id).IsRequired().ValueGeneratedOnAdd();

            // Name
            builder.Property(f => f.Name).IsRequired();

            // Budget
            builder.HasIndex(f => f.BudgetId);
            builder.Property(f => f.BudgetId).IsRequired();
            builder.HasOne(f => f.Budget)
                   .WithMany(f => f.BudgetCategories)
                   .OnDelete(DeleteBehavior.Cascade);
            
            
        }
    }
    
}
