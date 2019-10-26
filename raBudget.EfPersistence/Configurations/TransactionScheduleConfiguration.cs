using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace raBudget.EfPersistence.Configurations
{
    class TransactionScheduleConfiguration : IEntityTypeConfiguration<TransactionSchedule>
    { 
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<TransactionSchedule> builder)
        {
            // TransactionScheduleId
            builder.HasKey(f => f.Id);
            builder.Property(f=>f.Id).IsRequired().ValueGeneratedOnAdd();

            
            //BudgetCategory
            builder.HasIndex(f => f.BudgetCategoryId);
            builder.Property(f => f.BudgetCategoryId).IsRequired();
            builder.HasOne(x => x.BudgetCategory)
                   .WithMany(x => x.TransactionSchedules)
                   .OnDelete(DeleteBehavior.Cascade);

            //CreatedByUser
            builder.HasIndex(f => f.CreatedByUserId);
            builder.Property(f => f.CreatedByUserId).IsRequired();
            builder.HasOne(x => x.CreatedByUser)
                   .WithMany(x => x.TransactionSchedules)
                   .OnDelete(DeleteBehavior.Cascade);

            // StartDate
            builder.HasIndex(f => f.StartDate);
            builder.Property(f => f.StartDate).IsRequired();


            //Frequency
            builder.Property(f => f.Frequency).IsRequired();

            //PeriodStep
            builder.Property(f => f.PeriodStep).IsRequired();

            //Description
            builder.Property(f => f.Description).IsRequired();

            //Amount
            builder.Property(f => f.Amount).IsRequired();

        }
    }
    
}
