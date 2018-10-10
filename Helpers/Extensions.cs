using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public static class Extensions
    {
        public static double TransactionsSum(this IEnumerable<BudgetCategory> categories)
        {
            return categories.SelectMany(x => x.Transactions).Sum(x => x.Amount);
        }

        public static double AllocationsSum(this IEnumerable<BudgetCategory> categories)
        {
            return categories.SelectMany(x => x.Allocations).Sum(x => x.Amount);
        }
    }
}