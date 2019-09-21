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
    public class BudgetShareFilterModel
    {
        public IEnumerable<int> BudgetShareIdFilter { get; set; }
        public Guid UserIdFilter { get; set; }
    }
}
