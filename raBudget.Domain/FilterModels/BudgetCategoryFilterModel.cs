using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Domain.FilterModels
{
    /// <summary>
    /// Model describing possible filtering and sorting operations. All parameters can be null
    /// </summary>
    public class BudgetCategoryFilterModel
    {
        public IEnumerable<int> CategoryIdFilter { get; set; }
        public eBudgetCategoryType? CategoryType { get; set; }
        public eDataOrder DataOrder { get; set; }
        public Func<BudgetCategory, IComparable> OrderBy { get; set; }
    }
}
