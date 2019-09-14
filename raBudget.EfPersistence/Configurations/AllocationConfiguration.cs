using System;
using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace raBudget.EfPersistence.Configurations
{
    class AllocationConfiguration : IEntityTypeConfiguration<Allocation>
    { 
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Allocation> builder)
        {
            // AllocationId
            builder.HasKey(f => f.AllocationId);
            builder.Property(f=>f.AllocationId).IsRequired().ValueGeneratedOnAdd();

            // AllocationDateTime
            builder.HasIndex(f => f.AllocationDateTime);
            builder.Property(f => f.AllocationDateTime).IsRequired();

            //BudgetCategory
            builder.HasIndex(f => f.BudgetCategoryId);
            builder.Property(f => f.BudgetCategoryId).IsRequired();
            builder.HasOne(x => x.BudgetCategory)
                   .WithMany(x => x.Allocations)
                   .HasForeignKey(x=>x.BudgetCategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            //CreatedByUser
            builder.Property(f => f.CreatedByUserId)
                   .IsRequired();
            builder.HasOne(f => f.CreatedByUser)
                   .WithMany(f => f.RegisteredAllocations)
                   .HasForeignKey(f => f.CreatedByUserId);

            //CreationDateTime
            builder.Property(f => f.CreationDateTime)
                   .HasDefaultValue(DateTime.Now)
                   .IsRequired()
                   .ValueGeneratedOnAdd();

            //Description
            builder.Property(f => f.Description).IsRequired();

        }
    }
    
}
