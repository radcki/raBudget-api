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
    public class TransactionScheduleFilterModel
    {
        public IEnumerable<int> TransactionScheduleIdFilter { get; set; }
        public IEnumerable<int> CategoryIdFilter { get; set; }
        public IEnumerable<Guid> CreatedByUserIdFilter { get; set; }

        public DateTime? StartDateStartFilter { get; set; }
        public DateTime? StartDateEndFilter { get; set; }

        public DateTime? EndDateStartFilter { get; set; }
        public DateTime? EndDateEndFilter { get; set; }

        public int? LimitResults { get; set; }
        public eBudgetCategoryType? CategoryType { get; set; }
        public eDataOrder DataOrder { get; set; }
        public Func<TransactionSchedule, IComparable> OrderBy { get; set; }
    }
}
