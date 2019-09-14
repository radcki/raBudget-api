using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace raBudget.EfPersistence.Configurations
{
    class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    { 
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            // TransactionId
            builder.HasKey(f => f.TransactionId);
            builder.Property(f=>f.TransactionId).IsRequired().ValueGeneratedOnAdd();

            // TransactionDateTime
            builder.HasIndex(f => f.TransactionDateTime);
            builder.Property(f => f.TransactionDateTime).IsRequired();

            //BudgetCategory
            builder.HasIndex(f => f.BudgetCategoryId);
            builder.Property(f => f.BudgetCategoryId).IsRequired();
            builder.HasOne(x => x.BudgetCategory)
                   .WithMany(x => x.Transactions)
                   .HasForeignKey(x=>x.BudgetCategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            //CreatedByUser
            builder.Property(f => f.CreatedByUserId).IsRequired();
            builder.HasOne(f => f.CreatedByUser)
                   .WithMany(f => f.RegisteredTransactions)
                   .HasForeignKey(f=>f.CreatedByUserId);

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
