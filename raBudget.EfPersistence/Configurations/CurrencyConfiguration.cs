﻿using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Enum;

namespace raBudget.EfPersistence.Configurations
{
    class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    { 
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            // ISO CurrencyCode
            builder.HasKey(f => f.CurrencyCode);

            builder.HasIndex(x => x.Code);
            builder.Property(x => x.Code).IsRequired();
            builder.Property(x => x.EnglishName).IsRequired();
            builder.Property(x => x.NativeName).IsRequired();
            builder.Property(x => x.Symbol).IsRequired();

            builder.Ignore(x => x.NumberFormat);

            // Seed data
            var currencies = Enum.GetValues(typeof(eCurrency))
                                 .Cast<eCurrency>()
                                 .Select(Currency.Get);
            // Currency
            builder.HasData(currencies);


        }
    }
    
}