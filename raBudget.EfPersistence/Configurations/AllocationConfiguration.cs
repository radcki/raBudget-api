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
            builder.HasKey(f => f. Id);
            builder.Property(f=>f.Id).IsRequired().ValueGeneratedOnAdd();

            // AllocationDateTime
            builder.HasIndex(f => f.AllocationDateTime);
            builder.Property(f => f.AllocationDateTime).IsRequired();

            //TargetBudgetCategory
            builder.HasIndex(f => f.TargetBudgetCategoryId);
            builder.Property(f => f.TargetBudgetCategoryId).IsRequired();
            builder.HasOne(x => x.TargetBudgetCategory)
                   .WithMany(x => x.TargetAllocations)
                   .HasForeignKey(x=>x.TargetBudgetCategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            //SourceBudgetCategory
            builder.HasIndex(f => f.SourceBudgetCategoryId);
            builder.Property(f => f.SourceBudgetCategoryId).IsRequired();
            builder.HasOne(x => x.SourceBudgetCategory)
                   .WithMany(x => x.SourceAllocations)
                   .HasForeignKey(x => x.SourceBudgetCategoryId)
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
