using System;

namespace raBudget.Domain.Enum
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
