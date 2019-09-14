using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using raBudget.EfPersistence.Contexts;
using raBudget.EfPersistence.Infrastructure;

namespace raBudget.EfPersistence
{
    class DbContextFactory : DbContextFactoryBase<DataContext>
    {
        /// <inheritdoc />
        protected override DataContext CreateNewInstance(DbContextOptions<DataContext> options)
        {
            return new DataContext(options);
        }
    }
}
