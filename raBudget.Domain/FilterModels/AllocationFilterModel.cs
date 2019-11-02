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
    public class AllocationFilterModel
    {
        public IEnumerable<int> AllocationIdFilter { get; set; }
        public IEnumerable<int> TargetCategoryIdFilter { get; set; }
        public IEnumerable<int> SourceCategoryIdFilter { get; set; }
        public IEnumerable<Guid> CreatedByUserIdFilter { get; set; }

        public DateTime? AllocationDateStartFilter { get; set; }
        public DateTime? AllocationDateEndFilter { get; set; }

        public DateTime? CreationDateStartFilter { get; set; }
        public DateTime? CreationDateEndFilter { get; set; }

        public int? LimitResults { get; set; }
        public eDataOrder DataOrder { get; set; }
        public Func<Allocation, IComparable> OrderBy { get; set; }
    }
}
