using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Enum
{
    [Flags]
    public enum eBudgetCategoryBalanceIncluded
    {
        None = 0,
        OverallBalance = 1,
        ThisMonth = 2,
        ThisYear = 3,
        Totals = 4,
        All = ~(-1 << 5)
    }
}
