using System;

namespace raBudget.Domain.Enum
{
    [Flags]
    public enum eDataOrder
    {
        Default = 0,
        Ascending = 1,
        Descending = 2
    }
}
