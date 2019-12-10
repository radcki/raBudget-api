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
    public class TransactionsFilterModel
    {
        public TransactionsFilterModel()
        {
            DataOrder = eDataOrder.Descending;
        }
        public IEnumerable<int> TransactionIdFilter { get; set; }
        public IEnumerable<int> CategoryIdFilter { get; set; }
        public IEnumerable<Guid> CreatedByUserIdFilter { get; set; }
        public IEnumerable<int> TransactionScheduleIdFilter { get; set; }

        public DateTime? TransactionDateStartFilter { get; set; }
        public DateTime? TransactionDateEndFilter { get; set; }

        public DateTime? CreationDateStartFilter { get; set; }
        public DateTime? CreationDateEndFilter { get; set; }

        public int? LimitCategoryTypeResults { get; set; }
        public eBudgetCategoryType? CategoryType { get; set; }
        public eDataOrder DataOrder { get; set; }
        public Func<Transaction, IComparable> OrderBy { get; set; }
    }
}
