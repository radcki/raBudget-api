using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace raBudget.EfPersistence.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<User>
    { 
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // TransactionScheduleId
            builder.HasKey(f => f.Id);
            builder.Property(f=>f.Id).IsRequired();

        }
    }
    
}
